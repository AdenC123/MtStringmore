using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// Handles the death of player, when in contact with particles 
    /// </summary>
    public class ExplosionParticleController : MonoBehaviour
    {
        public ParticleSystem explosionParticles;

        private void Start()
        {
            explosionParticles = GetComponent<ParticleSystem>();

            if (explosionParticles != null)
            {
                var collisionModule = explosionParticles.collision;
                collisionModule.enabled = true;
                collisionModule.collidesWith = LayerMask.GetMask("Player", "Terrain");
            }
            else
            {
                Debug.LogError("ParticleSystem not found on this GameObject.");
            }
        }

        void OnParticleCollision(GameObject other)
        {
            if (other.CompareTag("Player"))
            {
                var playerController = other.GetComponent<PlayerController>();
                if (playerController.PlayerState != PlayerController.PlayerStateEnum.Dead)
                {
                    playerController.HandleDeath();
                }
            }
        }
    }
}