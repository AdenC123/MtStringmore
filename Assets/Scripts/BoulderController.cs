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
    [SerializeField] public float splitVelocity = 5f;
    
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
            SplitIntoSmallerBoulders();
            Destroy(gameObject);
        }
    }

    private void SplitIntoSmallerBoulders()
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject smallerBoulder = Instantiate(smallerBoulderPrefab, transform.position, Quaternion.identity);
            Rigidbody2D smallerRb = smallerBoulder.GetComponent<Rigidbody2D>();

            float randomAngle = (i == 0) ? 30f : -30f;
            float angleInRadians = randomAngle * Mathf.Deg2Rad;

            Vector2 velocity = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians)) * splitVelocity;
            smallerRb.velocity = velocity;

            float randomGravityScale = Random.Range(minGravityScale, maxGravityScale);
            smallerRb.gravityScale = randomGravityScale;
        }
    }
}