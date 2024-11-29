using UnityEngine;

namespace DefaultNamespace
{
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
            Debug.Log("entered OnParticleCollision");
            if (other.CompareTag("Player"))
            {
                Debug.Log("Player hit by explosion particle!");
                PlayerController playerController = other.GetComponent<PlayerController>();
                if (playerController.PlayerState != PlayerController.PlayerStateEnum.Dead)
                {
                    playerController.HandleDeath();
                }
            }
        }
    }
}