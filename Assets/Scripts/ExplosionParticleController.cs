using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// Handles the death of player, when in contact with particles 
    /// </summary>
    public class ExplosionParticleController : MonoBehaviour
    {
        [Range(0, 2)]
        [SerializeField] public float emissionRadius = 1.0f;
        [SerializeField] private ParticleSystem explosionParticles;

        private void Start()
        {
            explosionParticles = GetComponent<ParticleSystem>();

            if (explosionParticles != null)
            {
                var shapeModule = explosionParticles.shape;
                shapeModule.radius = emissionRadius;

                var collisionModule = explosionParticles.collision;
                collisionModule.enabled = true;
                collisionModule.collidesWith = LayerMask.GetMask("Player", "Terrain");
            }
        }

        private void Update()
        {
            if (explosionParticles != null)
            {
                var shapeModule = explosionParticles.shape;
                shapeModule.radius = emissionRadius;
            }
        }
    }
}