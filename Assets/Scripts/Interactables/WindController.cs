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
        [SerializeField] [Range(0f, 2f)] private float windStrength;
        [SerializeField] [Range(30f, 40f)] private float maxPlayerSpeed;
        [SerializeField] [Range(5f, 15f)] private float minPlayerSpeed;
        [SerializeField] private Vector2 windDirection;
        [SerializeField] private ParticleSystem windParticles;
        [SerializeField] private BoxCollider2D boxCollider;
        [SerializeField] private ParticleSystem childParticleSystem;
        
        #endregion
       
        private PlayerController _player;

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

        public bool IgnoreGravity => false;

        /// <inheritdoc />
        public Vector2 ApplyVelocity(Vector2 velocity)
        {
            // Don't apply wind during swing or dash
            if (_player == null || 
                _player.PlayerState == PlayerController.PlayerStateEnum.Dash || 
                _player.PlayerState == PlayerController.PlayerStateEnum.Swing)
            {
                return velocity;
            }

            Debug.Log("pre " + velocity);

            Vector2 windDir = windDirection.normalized;
            float dotProduct = Vector2.Dot(velocity, windDir);

            float windDelta = windStrength * Time.fixedDeltaTime * 10f;

            if (dotProduct < 0) // Moving against the wind
            {
                velocity += windDir * windDelta;

                float newDot = Vector2.Dot(velocity, windDir);
                float clampedDot = Mathf.Clamp(newDot, -minPlayerSpeed, 0f);

                // Rebuild velocity vector with clamped component along windDir
                Vector2 velocityAlongWind = windDir * clampedDot;
                Vector2 velocityPerpendicular = velocity - windDir * newDot;
                velocity = velocityAlongWind + velocityPerpendicular;
            }
            else if (dotProduct > 0) // Moving with the wind
            {
                velocity += windDir * windDelta * 10f;

                float newDot = Vector2.Dot(velocity, windDir);
                float clampedDot = Mathf.Clamp(newDot, 0f, maxWindSpeed);

                Vector2 velocityAlongWind = windDir * clampedDot;
                Vector2 velocityPerpendicular = velocity - windDir * newDot;
                velocity = velocityAlongWind + velocityPerpendicular;
            }

            Debug.Log("post " + velocity);
            return velocity;
        }


        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.TryGetComponent(out _player)) return;
            _player.AddPlayerVelocityEffector(this, true);
            _player.CanDash = true;
            _player.ForceCancelEarlyRelease();
            if (_player.PlayerState == PlayerController.PlayerStateEnum.Dash)
            {
                _player.ForceCancelDash();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.TryGetComponent(out _player)) return;
            _player.RemovePlayerVelocityEffector(this);
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
            parentShape.scale = new Vector3(boxCollider.size.x, boxCollider.size.y, parentShape.scale.z); 
            
            var childShape = childParticleSystem.shape;
            childShape.scale = parentShape.scale;
        }
    }
}