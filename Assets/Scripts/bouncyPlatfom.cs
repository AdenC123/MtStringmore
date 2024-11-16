using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class bouncyPlatfom : MonoBehaviour
{
    #region Serialized Private Fields
    [Header("Bouncing")] 
    [SerializeField] private float yBounceForce;
    [SerializeField] private float xBounceForce;
    #endregion
    
    private PlayerController playerController;
    
     private void OnCollisionEnter2D(Collision2D collision) 
    {
        // Check if the other object has the "Player" tag
        if (collision.gameObject.CompareTag("Player"))
        {
            // Do something when the object collides with the player
            Debug.Log("Player has collided with bouncy platform!");
            Debug.Log(playerController.Velocity);
            Vector2 directionVector = Vector2.Reflect(playerController.Velocity, collision.contacts[0].normal);
            Debug.Log(directionVector);
            if(playerController.Velocity.x <0 && directionVector.x <0 && playerController.Velocity.y<0 &&directionVector.y <0 ) {
                playerController.Velocity = new Vector2(-xBounceForce,yBounceForce);
            }
            else
            {
                playerController.Velocity = new Vector2(xBounceForce * Mathf.Sign(directionVector.x), 
                    yBounceForce * Mathf.Sign(directionVector.y));
            }
            Debug.Log(playerController.Velocity);
        }
       
    }
    
    // private void OnTriggerEnter2D(Collider2D collision)
    // {
    //     if (collision.gameObject.CompareTag("Player"))
    //     {
    //         playerController.Velocity = new Vector2(xBounceForce*Mathf.Sign(playerController.Velocity.x),yBounceForce);
    //     }
    // }
         
    void Start() {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
}
