using Player;
using UnityEngine;
namespace Interactables
{
    [RequireComponent(typeof(ParticleSystem))]
    [RequireComponent(typeof(BoxCollider2D))]
    [ExecuteAlways]
    public class WindController : MonoBehaviour, IPlayerVelocityEffector
    {
        #region Serialized Public Fields

        [Header("Wind Settings")]
        [SerializeField] [Range(0f, 150f)] private float windStrength;
        [SerializeField] [Range(0f, 150f)] private float maxWindSpeed;
        [SerializeField] private Vector2 windDirection;
        [SerializeField] private ParticleSystem windParticles;
        [SerializeField] private BoxCollider2D boxCollider;
        [SerializeField] private ParticleSystem childParticleSystem;
        
        #endregion
       
        private PlayerController _player;
        private bool _inWindZone;

        // Called automatically in editor when a serialized field changes
        private void OnValidate()
        {
            UpdateParticles();
        }

        public void Start()
        {
            UpdateParticles();
        }

        /// <inheritdoc />
        /// <remarks>
        /// Wind will not change player velocity if on swing or trampoline etc
        /// </remarks>
        public bool IgnoreOtherEffectors => false;

        /// <inheritdoc />
        public Vector2 ApplyVelocity(Vector2 velocity)
        {
            Debug.Log("Wind apply velocity called with velocity: " + velocity);
            // Don't apply wind during swing or dash
            if (_player == null || _player.PlayerState == PlayerController.PlayerStateEnum.Dash || _player.PlayerState == PlayerController.PlayerStateEnum.Swing)
            {
                return velocity;
            }

            Vector2 windDir = windDirection.normalized;
            float currentSpeedInWindDir = Vector2.Dot(velocity, windDir);
            if (currentSpeedInWindDir < maxWindSpeed)
            {   
                float boostAmount = windStrength * Time.fixedDeltaTime * 10;
                Debug.Log("Pre Vel: "+ velocity);
                velocity += windDir * boostAmount;
                Debug.Log("Post Vel: "+ velocity);
                // remove excess if overshoot
                float newSpeedInWindDir = Vector2.Dot(velocity, windDir);
                if (newSpeedInWindDir > maxWindSpeed)
                {
                    float overshoot = newSpeedInWindDir - maxWindSpeed;
                    velocity -= windDir * overshoot;
                }

            }

            return velocity;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out _player)) return;
            _player.AddPlayerVelocityEffector(this, true);
            _player.CanDash = true;
            _player.ForceCancelEarlyRelease();
            if (_player.PlayerState == PlayerController.PlayerStateEnum.Dash)
            {
                _player.ForceCancelDash();
            }
            _inWindZone = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.TryGetComponent(out _player)) return;
            _player.RemovePlayerVelocityEffector(this);
            _inWindZone = false;
        }

        // Adjust the particle system based on wind direction and speed
        private void UpdateParticles()
        {
            if (windParticles == null || boxCollider == null ) return;
            
            // Determine the equivalent angle of the wind based on vector direction
            float angle = Mathf.Atan2(windDirection.normalized.y, windDirection.normalized.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);

            // Possibly loop through all children (if multiple particle layers)
            Transform childTransform1 = transform.GetChild(0);
            childTransform1.rotation =  Quaternion.Euler(0f, 0f, angle);

            //var windMain = windParticles.main;
            // tweak this magic number eventually
            // if (windStrength > 100) windMain.simulationSpeed = 2;
            
            var parentShape = windParticles.shape;
            parentShape.scale = new Vector3(boxCollider.size.x, boxCollider.size.y, parentShape.scale.z); // preserve z if needed
            
            var childShape = childParticleSystem.shape;
            childShape.scale = parentShape.scale;
        }
    }
}