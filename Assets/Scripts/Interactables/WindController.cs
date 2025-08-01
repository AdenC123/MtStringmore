using Player;
using UnityEngine;
namespace Interactables
{
    [RequireComponent(typeof(BoxCollider2D), typeof(ParticleSystem), typeof(AudioSource))]
    [ExecuteAlways]
    public class WindController : MonoBehaviour, IPlayerVelocityEffector
    {
        #region Serialized Private Fields

        [Header("Wind Settings")]
        [SerializeField, Tooltip("Player speed moving with wind")] private float tailwindSpeed;
        [SerializeField, Tooltip("Player speed moving against wind")] private float headwindSpeed;
        [SerializeField] private Vector2 windDirection;
        [SerializeField, Tooltip("Updates both collider2D size and particle region size")] private Vector2 windZoneSize;  
        [SerializeField] private ParticleSystem windParticles;
        [SerializeField] private BoxCollider2D boxCollider;
        [SerializeField] private ParticleSystem childParticleSystem;
        [Header("Sounds")] 
        [SerializeField, Tooltip("How quickly the wind audio fades")] private float fadeSpeed;
        [SerializeField] private AudioSource audioSource;
        #endregion
       
        #region Private Properties

        private PlayerController _player;
        private bool _playerInside;
        private bool _shouldFadeOut;
        private Vector2 _windDirNormalized;

        #endregion
        // Called automatically in editor when a serialized field changes
        private void OnValidate()
        {
            _windDirNormalized = windDirection.normalized;
            UpdateParticles();
        }

        private void Awake()
        {
            _windDirNormalized = windDirection.normalized;
            UpdateParticles();
        }

        private void Update()
        {
            if (audioSource == null) return;

            float targetVolume = _playerInside ? 1f : 0f;
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, fadeSpeed * Time.deltaTime);

            // Stop audio when fully faded out
            if (!_playerInside && _shouldFadeOut && Mathf.Approximately(audioSource.volume, 0f))
            {
                audioSource.Stop();
                _shouldFadeOut = false; 
            }
        }

        /// <inheritdoc />
        /// <remarks>
        /// Wind will not change player velocity if on swing or trampoline etc
        /// </remarks>
        public bool IgnoreOtherEffectors => false;

        public bool IgnoreGravity => true;

        /// <inheritdoc />
        public Vector2 ApplyVelocity(Vector2 velocity)
        {
            // Don't apply wind during swing or dash
            if (_player == null || _player.PlayerState == PlayerController.PlayerStateEnum.Swing || _player.PlayerState == PlayerController.PlayerStateEnum.OnObject)
            {
                return velocity;
            }
            
            float dotProduct = Vector2.Dot(velocity.normalized, _windDirNormalized);
            // Case where player's jump height was massively reduced
            if (_windDirNormalized.y == 0)
            {
                if (dotProduct <= 0) // Moving against the wind
                {
                    Vector2 resistance = _windDirNormalized * headwindSpeed;
                    velocity.x = Mathf.MoveTowards(velocity.x, resistance.x, Time.fixedDeltaTime * 20f);
                }
                else if (dotProduct > 0) // Moving with the wind
                {
                    Vector2 boost = _windDirNormalized * tailwindSpeed;
                    velocity.x = Mathf.MoveTowards(velocity.x, boost.x, Time.fixedDeltaTime * 50f);
                }
            }
            // Case where diagonal wind tunnels are needed
            else
            {
                if (dotProduct <= 0) // Moving against the wind
                {
                    Vector2 resistance = _windDirNormalized * headwindSpeed;
                    velocity = Vector2.MoveTowards(velocity, resistance, Time.fixedDeltaTime * 20f);
                }
                else if (dotProduct > 0) // Moving with the wind
                {
                    Vector2 boost = _windDirNormalized * tailwindSpeed;
                    velocity = Vector2.MoveTowards(velocity, boost, Time.fixedDeltaTime * 50f);
                }
            }
            return velocity;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("Entered Wind Controller");
            if (!other.TryGetComponent(out PlayerController player)) return;
            _playerInside = true;
            audioSource.Play();
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.TryGetComponent(out PlayerController player)) return;
            _player = player; // reassigns every frame to fix loss of player instance on death
            _player.AddPlayerVelocityEffector(this, true);
            // _player.CanDash = true;
            _player.ForceCancelEarlyRelease();
            // if (_player.PlayerState == PlayerController.PlayerStateEnum.Dash)
            // {
            //     _player.ForceCancelDash();
            // }
            float targetVolume = _playerInside ? 1f : 0f;
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, fadeSpeed * Time.deltaTime);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            Debug.Log("Entered Wind Controller");
            if (!other.TryGetComponent(out PlayerController _player)) return;
            _player.RemovePlayerVelocityEffector(this);
            _playerInside = false;
            _shouldFadeOut = true;
        }


        // Adjust the particle system based on wind direction and speed
        private void UpdateParticles()
        {
            if (windParticles == null || boxCollider == null ) return;
            
            // Determine the equivalent angle of the wind based on vector direction
            float angle = Mathf.Atan2(windDirection.normalized.y, windDirection.normalized.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);

            // Line up the angle the snow should blow in based on wind
            Transform childTransform1 = transform.GetChild(0);
            childTransform1.rotation =  Quaternion.Euler(0f, 0f, angle);

            // Ensure box collider and particle systems shape match up
            Vector2 newScale = new Vector2(windZoneSize.x, windZoneSize.y); 
            var parentShape = windParticles.shape;
            parentShape.scale = newScale;

            var childShape = childParticleSystem.shape;
            childShape.scale = newScale;

            boxCollider.size =  newScale;
        }
    }
}
