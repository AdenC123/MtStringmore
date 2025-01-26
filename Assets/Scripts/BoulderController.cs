using System;
using UnityEngine;
using Random = UnityEngine.Random;


/// <summary>
/// Handles the physics and features behind the boulders 
/// </summary>
public class BoulderController : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private ParticleSystem liquidMolecule;
    [SerializeField] public bool toExplode = false;
    
    [Range(0, 5)]
    [SerializeField] public float minGravityScale = 3f;
    [Range(0, 5)]
    [SerializeField] public float maxGravityScale = 5f;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        float randomGravityScale = Random.Range(minGravityScale, maxGravityScale);
        rb.gravityScale = randomGravityScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            if (toExplode)
            {
                TurnIntoParticles();
            }
            Destroy(gameObject);
        }
    }

    private void TurnIntoParticles()
    {
        if (liquidMolecule != null)
        {
            float yOffset = 0.5f;

            // Set the particle system position to just above the boulder, so the animation looks better
            Vector3 particlePosition = transform.position + new Vector3(0, yOffset, 0);

            // physically move the particles to the newly created position
            ParticleSystem particles = Instantiate(liquidMolecule, particlePosition, Quaternion.identity);

            particles.Play();

            Destroy(particles.gameObject, particles.main.duration + particles.main.startLifetime.constantMax);
        }
    }

}