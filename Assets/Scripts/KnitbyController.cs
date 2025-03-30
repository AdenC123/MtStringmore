using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Updates Knitby's position to follow the player's path
/// </summary>
public class KnitbyController : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private GameObject deathSmoke;

    [Header("Follow Settings")] [SerializeField]
    private float timeOffset = 0.1f;

    [SerializeField] private int granularity = 10;
    [SerializeField] private float interpolationSpeed = 20;

    [Header("Collisions")] [SerializeField]
    private LayerMask collisionLayer;

    [SerializeField] private float collisionDistance;
    private readonly Queue<Vector3> _path = new();
    private CapsuleCollider2D _col;
    private Vector3 _currentPathPosition;
    private bool _grounded;
    private LineRenderer _lineRenderer;


    private GameObject _player;
    private float _queueTimer;

    private void Start()
    {
        _col = GetComponent<CapsuleCollider2D>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _lineRenderer = _player.GetComponentInChildren<LineRenderer>();
        PlayerController playerController = _player.GetComponent<PlayerController>();
        playerController.Death += PlayerDeath;
    }

    private void Update()
    {
        if (_currentPathPosition == Vector3.zero) return;
        SetIdle?.Invoke(Vector3.Distance(transform.position, _currentPathPosition) <= 0.01);
        Vector3 direction = _currentPathPosition - transform.position;

        DirectionUpdated?.Invoke(direction.x, direction.y);
        transform.position += direction * (Time.deltaTime * interpolationSpeed);
    }

    private void FixedUpdate()
    {
        // granularity controls how many path positions can be stored in the queue before it is dequeued
        // this controls how far back we want this game object to lag behind the objectToFollow
        if (_player is null)
            return;
        _queueTimer -= Time.fixedDeltaTime;

        if (!(_queueTimer <= 0)) return;
        _queueTimer = timeOffset / granularity;
        if (_path.Count == granularity)
            _currentPathPosition = _path.Dequeue();
        _path.Enqueue(_player.transform.position);

        bool groundHit = CapsuleCastCollision(Vector2.down, collisionDistance);
        if (_grounded != groundHit)
        {
            _grounded = groundHit;
            GroundedChanged?.Invoke(groundHit);
        }

        Swing?.Invoke(_lineRenderer.isVisible);
    }

    private void OnDisable()
    {
        if (_player == null || _player) return;
        PlayerController playerController = _player.GetComponent<PlayerController>();
        playerController.Death -= PlayerDeath;
    }

    public event Action<bool> SetIdle;

    /// <summary>
    ///     Fires when direction updates.
    ///     Parameters: x and y, distances from position in previous frame
    /// </summary>
    public event Action<float, float> DirectionUpdated;

    /// <summary>
    ///     Fires when Knitby becomes grounded or leaves the ground.
    ///     Parameters:
    ///     bool: false if leaving the ground, true if becoming grounded
    ///     float: player's Y velocity
    /// </summary>
    public event Action<bool> GroundedChanged;

    /// <summary>
    ///     Fires continuously; true when currently in swing, false otherwise
    /// </summary>
    public event Action<bool> Swing;

    /// <summary>
    ///     Fires when player is dead
    /// </summary>
    public event Action PlayerDeath;

    private RaycastHit2D CapsuleCastCollision(Vector2 dir, float distance)
    {
        return Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0,
            dir, distance, collisionLayer);
    }
}
