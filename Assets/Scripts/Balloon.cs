using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Balloon : AbstractPlayerInteractable
{
    [SerializeField, Tooltip("Acceleration curve over time, in [0, 1]")]
    private AnimationCurve accelerationCurve;
    
    [SerializeField, Tooltip("Player offset from balloon")]
    private Vector3 offset = new(0, -3, 0);

    [SerializeField, Min(0), Tooltip("Minimum speed")]
    private float minSpeed = 1;

    [SerializeField, Min(0), Tooltip("Maximum speed")]
    private float maxSpeed = 1;

    [SerializeField, Min(0), Tooltip("Acceleration time (seconds)")]
    private float accelerationTime = 1;
 
    [SerializeField, Min(0), Tooltip("Additional velocity boost on exit")]
    private Vector2 exitVelBoost = new(10, 10);
    
    [SerializeField, Tooltip("Distance to ensure player does not clip into ground")]
    private float attachRayDistance = 0.1f;
    
    [SerializeField, Tooltip("Access to groundLayer to check attach requirements")]
    private LayerMask groundLayerMask;
    
    [SerializeField, Tooltip("Access to Animator")]
    private Animator playerAnimator;
    
    [SerializeField, Tooltip("Access to Player Sound Effect")]
    private AudioSource playerAudio;
    
    [SerializeField, Tooltip("Boolean to check if the player is still on the interactable object")]
    private bool isPlayerInteractable;

    /// <remarks>
    /// Has to be public to allow the editor to modify this without reflection.
    /// </remarks>
    [Tooltip("First position (world space)")]
    public Vector2 positionOffset;

    private Vector2 firstPosition;
    private Vector2 secondPosition;
    
    /// <remarks>
    /// I know the new reset logic hasn't been merged in yet,
    /// but we need to save a copy of the enumerator to reset the object later.
    /// </remarks>
    private Coroutine _activeMotion;
    
    //Retrieves balloon's rigidbody on awake
    private Rigidbody2D _rigidbody;
    private PlayerController _player;
    
    [SerializeField, Tooltip("Allowed error of player to balloon before respawning")]
    private float positionTolerance = 0.1f;
    private bool playerAttached = false;
    
    [SerializeField, Tooltip("Variable for how long the boost lasts after jumping off")]
    private float boostTimer;
    private bool isBoosted;

    
    /// <inheritdoc />
    public override bool IgnoreGravity => true;

    /// <summary>
    /// Returns the time of the last keyframe.
    /// </summary>
    /// <remarks>
    /// Should be 1, but just in case there's user error, we scale anyways.
    /// </remarks>
    private float LastKeyframeTime
    {
        get
        {
            Keyframe[] keys = accelerationCurve.keys;
            if (keys == null || keys.Length == 0) return 1;
            return keys[^1].time;
        }
    }

    /// <summary>
    /// Returns the distance along the path.
    /// </summary>
    /// <remarks>
    /// Since people may or may not adjust the position in the editor while moving,
    /// this computes the vector projection along the actual path in case someone changes the direction while running.
    /// </remarks>
    private float DistanceAlongPath
    {
        get
        {
            Vector2 direction = secondPosition - firstPosition;
            Vector2 travelled = _rigidbody.position - firstPosition;
            return Vector2.Dot(direction, travelled) / direction.magnitude;
        }
    }

    /// <summary>
    /// Evaluates the velocity at a specific time since motion start.
    /// </summary>
    /// <param name="time">Time since motion start</param>
    /// <returns>Velocity at time</returns>
    private float EvaluateAt(float time)
    {
        return Mathf.Lerp(minSpeed, maxSpeed, accelerationCurve.Evaluate(LastKeyframeTime * time / accelerationTime));
    }


    /// <summary>
    /// Coroutine to move the object according to the motion curve.
    /// </summary>
    /// <returns></returns>
    private IEnumerator MotionCoroutine()
    {
        // i can't guarantee that the end user *won't* change the positions during runtime
        // so I have to check *literally every frame*.
        float time = 0;
        for (Vector2 diff = secondPosition - firstPosition;
             DistanceAlongPath <= diff.magnitude;
             diff = secondPosition - firstPosition)
        {
            yield return new WaitForFixedUpdate();
            // yes, I could use possibly use FixedJoint2D, but I suspect that PlayerController may cause problems
            
            // respawn logic, if balloon reaches second position w/o player, then respawn balloon in start position
            if (Vector2.Distance(_rigidbody.position, secondPosition) < positionTolerance && !playerAttached)
            {
                RespawnBalloon();
                yield break;
            }
            
            _rigidbody.velocity = EvaluateAt(time) * diff.normalized;
            time += Time.fixedDeltaTime;
        }

        _rigidbody.position = secondPosition;
        _rigidbody.velocity = Vector2.zero;
    }

    /// <summary>
    /// Starts moving.
    /// </summary>
    private void StartMotion()
    {
        _activeMotion ??= StartCoroutine(MotionCoroutine());
        isPlayerInteractable = true;
    }

    /// <summary>
    /// Stops moving.
    /// </summary>
    private void StopMotion()
    {
        if (_activeMotion != null) StopCoroutine(_activeMotion);
        _activeMotion = null;
        // defensive check to make sure velocity gets set to 0
        // we only want to set velocity to zero after applying previous velocity.
        if (!ReferenceEquals(_player.ActiveVelocityEffector, this))
        {
            _rigidbody.velocity = Vector2.zero;
        }
    }

    /// <inheritdoc />
    public override void OnPlayerEnter(PlayerController player)
    {
        playerAttached = true;
        if (_rigidbody.position == secondPosition)
        {
            // disallow re-attaching if reached
            player.CurrentInteractableArea = null;
        } else
        {
            player.CurrentInteractableArea = this;
        }
    }

    /// <inheritdoc />
    public override void OnPlayerExit(PlayerController player)
    {
        playerAttached = false;
        if (Vector2.Distance(_rigidbody.position, secondPosition) < positionTolerance) RespawnBalloon();
    }

    /// <inheritdoc />
    public override Vector2 ApplyVelocity(Vector2 velocity)
    {
        Vector2 balloonVelocity = _rigidbody.velocity;

        if (!isPlayerInteractable)
        {
            if (ReferenceEquals(_player.ActiveVelocityEffector, this))
                _player.ActiveVelocityEffector = null;

            // Use the balloon's velocity to determine the exit boost direction
            Vector2 boostDirection = balloonVelocity.normalized;
            if (boostDirection == Vector2.zero) // Fallback in case balloon is stationary
                boostDirection = Vector2.up; // Default to an upward boost

            Vector2 boost = new(_player.Direction * exitVelBoost.x, boostDirection.y * exitVelBoost.y);
            return boost; // Apply only the boost since player is stationary
        }
        
        return balloonVelocity;
    }
    
    /// <summary>
    /// Ensures character will not clip into the ground when attaching to balloon.
    /// </summary>
    /// <inheritdoc />
    public override void StartInteract(PlayerController player)
    {
        _player = player;
        playerAnimator.enabled = false; //to-do, set to balloon anim
        playerAudio.enabled = false; //to-do, set to balloon sfx
        
        Vector2 targetPosition  = (Vector2)transform.position + (Vector2)offset;
        if (CanAttachAtPosition(targetPosition))
        {
            player.transform.position = targetPosition;
            player.ActiveVelocityEffector = this;
            StartMotion();
        }
        else
        {
            Debug.Log("Cannot attach, distance to ground too small");
        }
    }
    
    private bool CanAttachAtPosition(Vector2 targetPosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(targetPosition, Vector2.down, attachRayDistance, groundLayerMask);

        // If the ray hits ground and it's too close, do not attach
        return hit.collider == null;
    }
    
    
    /// <inheritdoc />
    public override void EndInteract(PlayerController player)
    {
        player.ActiveVelocityEffector = null;
        playerAnimator.enabled = true;
        playerAudio.enabled = true; // to-do, handle the balloon sound effect ending here
        isPlayerInteractable = false;   
        isBoosted = true;

        // if (player.TryGetComponent(out Rigidbody2D playerRb)) {
        //     Vector2 boostedVelocity = ApplyVelocity(playerRb.velocity);
        //     Debug.Log($"Applying exit boost: {boostedVelocity}");
        //     playerRb.AddForce(boostedVelocity);
        // }
    }

    private void FixedUpdate()
    {
        if (_player &&
            !isPlayerInteractable &&
            _player.TryGetComponent(out Rigidbody2D playerRb) &&
            isBoosted)
        {
            Vector2 boostedVelocity = ApplyVelocity(playerRb.velocity);
            playerRb.velocity += new Vector2(boostedVelocity.x, boostedVelocity.y);
            StartCoroutine(ResetBoostAfterTime(playerRb, boostTimer));
        }
    }

    private IEnumerator ResetBoostAfterTime(Rigidbody2D playerRb, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Keep applying the horizontal velocity boost (preserving Y motion)
            playerRb.velocity = new Vector2(playerRb.velocity.x, playerRb.velocity.y);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        isBoosted = false;
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        firstPosition = new Vector2(transform.position.x, transform.position.y);
        secondPosition = firstPosition + positionOffset;
    }

    private void OnValidate()
    {
        foreach (Keyframe key in (accelerationCurve?.keys ?? Array.Empty<Keyframe>()))
        {
            if (key.value is < 0 or > 1)
            {
                Debug.LogWarning($"Acceleration curve keyframe is out of range: {key.time}, {key.value}");
            }
        }

        if (!Mathf.Approximately(LastKeyframeTime, 1))
        {
            Debug.LogWarning(
                $"Acceleration curve last keyframe time is not 1; time will be scaled: {LastKeyframeTime}");
        }

        Rigidbody2D body = GetComponent<Rigidbody2D>();
        if (body?.interpolation != RigidbodyInterpolation2D.Interpolate)
        {
            Debug.LogWarning("Rigidbody isn't interpolated: positions may appear invalid while moving!");
        }

        if (!body?.isKinematic ?? false)
        {
            Debug.LogWarning("Rigidbody isn't kinematic: may cause problems!");
        }

        if (body)
        {
            List<RaycastHit2D> hits = new();
            body.position = firstPosition;
            body.Cast((secondPosition - firstPosition).normalized, hits, (secondPosition-firstPosition).magnitude);
            foreach (RaycastHit2D hit in hits)
            {
                // some wack things may happen if the player collides with something while moving
                Debug.LogWarning("Object may be in motion path: " + hit.transform.gameObject.name);
            }
        }
    }
    
    private void RespawnBalloon()
    {
        playerAttached = false;
        _rigidbody.position = firstPosition;
        StopMotion();
        Debug.Log("Balloon has respawned at the first position.");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(firstPosition, 1);
        Gizmos.DrawLine(firstPosition, secondPosition);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(secondPosition, 1);
    }
}
