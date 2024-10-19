using System;
using UnityEngine;
using Random = UnityEngine.Random;


/// <summary>
/// Handles the physics and features behind the boulders 
/// </summary>
public class BoulderController : MonoBehaviour
{
    private Rigidbody2D rb;
    
    [Range(-100, 0)]
    [SerializeField] public float minXVelocity = -20f;
    [Range(-100, 0)]
    [SerializeField] public float maxXVelocity = 0f;
    [Range(1, 5)]
    [SerializeField] public float minGravityScale = 1f;
    [Range(1, 5)]
    [SerializeField] public float maxGravityScale = 2f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
            
        float randomXVelocity = Random.Range(minXVelocity, maxXVelocity);
        rb.velocity = new Vector2(randomXVelocity, rb.velocity.y);
            
        float randomGravityScale = Random.Range(minGravityScale, maxGravityScale);
        rb.gravityScale = randomGravityScale;
    }

    // can implement this class to handle what happens to the boulder when it hits another object
    // implement the shattering of the boulder
    private void OnTriggerEnter2D(Collider2D other)
    {
        throw new NotImplementedException();
    }
}