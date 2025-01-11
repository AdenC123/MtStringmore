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
    
    [SerializeField] public GameObject smallerBoulderPrefab;
    [SerializeField] public float splitVelocity = 8f;
    [SerializeField] public bool toExplode = false;
    
    [Range(1, 5)]
    [SerializeField] public float minGravityScale = 3f;
    [Range(1, 5)]
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
                // SplitIntoSmallerBoulders();
                TurnIntoParticles();
            }
            Destroy(gameObject);
            
        }
    }

    private void SplitIntoSmallerBoulders()
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject smallerBoulder = Instantiate(smallerBoulderPrefab, transform.position, Quaternion.identity);
            Rigidbody2D smallerRb = smallerBoulder.GetComponent<Rigidbody2D>();
    
            float randomAngle = (i == 0) ? 45f : -45f;
            float angleInRadians = randomAngle * Mathf.Deg2Rad;
    
            Vector2 velocity = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians)) * splitVelocity;
            smallerRb.velocity = (i == 0) ? velocity : -velocity;
    
            // float randomGravityScale = Random.Range(minGravityScale, maxGravityScale);
            // smallerRb.gravityScale = randomGravityScale;
            
            Destroy(smallerBoulder, 1f);
        }
    }

    private void TurnIntoParticles()
    {
        if (liquidMolecule != null)
        {
            // liquidMolecule.transform.position = transform.position;
            float yOffset = 0.5f; // Adjust this value as needed
            
            // Set the particle system position to just above the boulder
            Vector3 particlePosition = transform.position + new Vector3(0, yOffset, 0);
            ParticleSystem particles = Instantiate(liquidMolecule, particlePosition, Quaternion.identity);
            
            particles.Play();
            
            Destroy(particles.gameObject, particles.main.duration + particles.main.startLifetime.constantMax);
        }
            
        // Destroy(gameObject, 0.1f);
    }

}