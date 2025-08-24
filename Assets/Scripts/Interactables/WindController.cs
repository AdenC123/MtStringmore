using System.Collections;
using Managers;
using Player;
using UnityEngine;

namespace Interactables
{
    /// <summary>
    /// Wind controller.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D), typeof(AudioSource))]
    public class WindController : MonoBehaviour, IPlayerVelocityEffector
    {
        #region Serialized Private Fields

        [Header("Wind Settings")]
        [SerializeField, Tooltip("Player speed moving with wind"), Min(0)] private float tailwindSpeed;
        [SerializeField, Tooltip("Player speed moving against wind"), Min(0)] private float headwindSpeed;
        [SerializeField, Tooltip("Player deceleration in headwind"), Min(0)] private float headwindDecl = 20f;
        [SerializeField, Tooltip("Player acceleration in tailwind"), Min(0)] private float tailwindAccel = 50f;
        [SerializeField] private ParticleSystem windParticles;
        [SerializeField] private ParticleSystem childParticleSystem;
        [Header("Sounds")] 
        [SerializeField, Tooltip("How quickly the wind audio fades"), Min(0)] private float fadeSpeed;
        #endregion
       
        #region Private Properties

        private PlayerController _player;
        private BoxCollider2D _boxCollider;
        private AudioSource _audioSource;
        private bool _playerInside;
        private Coroutine _fadeOutCoroutine;
        private Coroutine _fadeInCoroutine;
        private Vector2 WindDirNormalized => transform.right.normalized;
        #endregion

        /// <inheritdoc />
        /// <remarks>
        /// Wind will not change player velocity if on swing or trampoline etc
        /// </remarks>
        public bool IgnoreOtherEffectors => false;
        
        /// <inheritdoc />
        public bool EffectPlayerWalkSpeed => true;
        /// <inheritdoc />
        public bool AllowPlayerDashing => true;

        private void OnValidate()
        {
            UpdateParticleSystemSize();
        }

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider2D>();
            _audioSource = GetComponent<AudioSource>();
            UpdateParticleSystemSize();
            GameManager.Instance.Reset += OnReset;
        }

        private void OnDestroy()
        {
            GameManager.Instance.Reset -= OnReset;
        }

        /// <summary>
        /// Called on reset — as the player on reset removes all velocity effectors,
        /// add it back if they're inside.
        /// </summary>
        private void OnReset()
        {
            if (_playerInside)
                _player.AddPlayerVelocityEffector(this);
        }

        /// <summary>
        /// Fades out the audio.
        /// </summary>
        /// <returns>Coroutine</returns>
        private IEnumerator FadeOutAudio()
        {
            for (float volume = _audioSource.volume; volume > 0; volume -= fadeSpeed * Time.deltaTime)
            {
                _audioSource.volume = volume;
                yield return null;
            }
            _audioSource.Stop();
            _fadeOutCoroutine = null;
        }

        /// <summary>
        /// Fades in the audio.
        /// </summary>
        /// <returns>Coroutine</returns>
        private IEnumerator FadeInAudio()
        {
            _audioSource.Play();
            for (float volume = _audioSource.volume; volume < 1; volume += fadeSpeed * Time.deltaTime)
            {
                _audioSource.volume = volume;
                yield return null;
            }

            _audioSource.volume = 1;
            _fadeInCoroutine = null;
        }

        /// <inheritdoc />
        public Vector2 ApplyVelocity(Vector2 velocity)
        {
            // Don't apply wind for specified player states
            if (!_player || _player.PlayerState == PlayerController.PlayerStateEnum.Swing || _player.PlayerState == PlayerController.PlayerStateEnum.OnObject)
            {
                return velocity;
            }
            
            float dotProduct = Vector2.Dot(velocity.normalized, WindDirNormalized);
            float windSpeed;
            float windRoc;
            
            if (dotProduct <= 0) // Moving against the wind
            {
                windSpeed = headwindSpeed;
                windRoc = headwindDecl;
            }
            else // Moving with the wind
            {
                windSpeed = tailwindSpeed;
                windRoc = tailwindAccel;
            }
            
            Vector2 target = WindDirNormalized * windSpeed;
            velocity = (WindDirNormalized.y == 0)
                ? new Vector2(Mathf.MoveTowards(velocity.x, target.x, Time.fixedDeltaTime * windRoc), velocity.y) 
                : Vector2.MoveTowards(velocity, target, Time.fixedDeltaTime * windRoc); 
            return velocity;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out PlayerController player)) return;
            _playerInside = true;
            if (_fadeOutCoroutine != null) StopCoroutine(_fadeOutCoroutine);
            _fadeOutCoroutine = null;
            _fadeInCoroutine ??= StartCoroutine(FadeInAudio());
            _player = player; 
            _player.AddPlayerVelocityEffector(this);
            _player.CanDash = true;
            _player.ForceCancelEarlyRelease();
            if (_player.PlayerState == PlayerController.PlayerStateEnum.Dash)
            {
                _player.ForceCancelDash();
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.TryGetComponent(out PlayerController player)) return;
            _player = player;
            _player.RemovePlayerVelocityEffector(this);
            _playerInside = false;
            if (_fadeInCoroutine != null) StopCoroutine(_fadeInCoroutine);
            _fadeInCoroutine = null;
            _fadeOutCoroutine = StartCoroutine(FadeOutAudio());
        }

        /// <summary>
        /// Ensures consistent sizing across particle systems and box collider.
        /// </summary>
        private void UpdateParticleSystemSize()
        {
            _boxCollider = GetComponent<BoxCollider2D>();
            if (!windParticles || !_boxCollider) return;

            // Ensure box collider and particle systems shape match up
            Vector2 newScale = _boxCollider.size;
            ParticleSystem.ShapeModule windParticlesShape = windParticles.shape;
            if ((Vector2)windParticlesShape.scale != newScale)
            {
                Debug.LogWarning($"Updating {windParticles.gameObject.name} particle system scale to be {newScale}");
                windParticlesShape.scale = newScale;
            }

            ParticleSystem.ShapeModule childShape = childParticleSystem.shape;
            if ((Vector2)childShape.scale != newScale)
            {
                Debug.LogWarning($"Updating {childParticleSystem.gameObject.name} particle system scale to be {newScale}");
                childShape.scale = newScale;
            }
        }
    }
}
