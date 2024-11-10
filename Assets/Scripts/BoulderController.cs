using System;
using UnityEngine;
using Random = UnityEngine.Random;


/// <summary>
/// Handles the physics and features behind the boulders 
/// </summary>
public class BoulderController : MonoBehaviour
{
    private Rigidbody2D rb;
    
    [SerializeField] public GameObject smallerBoulderPrefab;
    [SerializeField] public GameObject tinyLiquidMoleculePrefab;
    [SerializeField] private ParticleSystem liquidMolecule;
    [SerializeField] public float splitVelocity = 8f;
    
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

    // can implement this class to handle what happens to the boulder when it hits another object
    // implement the shattering of the boulder
    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     throw new NotImplementedException();
    // }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            // SplitIntoSmallerBoulders();
            TurnIntoLiquid();
            
            // TurnIntoLiquidParticleSystem();
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
    
    private void TurnIntoLiquid()
    {
        int numberOfFragments = 50;
        float minFragmentVelocity = splitVelocity * 0.5f;
        float maxFragmentVelocity = splitVelocity * 1.5f;

        for (int i = 0; i < numberOfFragments; i++)
        {
            GameObject smallerBoulder = Instantiate(smallerBoulderPrefab, transform.position, Quaternion.identity);
            // adjust molecule size as needed
            smallerBoulder.transform.localScale *= 0.3f;
            Rigidbody2D smallerRb = smallerBoulder.GetComponent<Rigidbody2D>();

            float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float randomVelocity = Random.Range(minFragmentVelocity, maxFragmentVelocity);

            Vector2 velocity = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) * randomVelocity;
            smallerRb.velocity = velocity;

            float randomGravityScale = Random.Range(minGravityScale, maxGravityScale);
            smallerRb.gravityScale = randomGravityScale;

            Destroy(smallerBoulder, 1f);
        }
    }

    private void TurnIntoLiquidParticleSystem()
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