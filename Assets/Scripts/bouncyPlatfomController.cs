using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Controls the bouncy platform physics 
/// </summary>
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
        if (collision.gameObject.CompareTag("Player"))
        {
            //on first contact determine the players new direction
            Vector2 directionVector = Vector2.Reflect(playerController.Velocity, collision.contacts[0].normal);

            //ensure the player bounces correctly when hitting the platform coming from the left
            if(playerController.Velocity.x <0 && directionVector.x <0 && playerController.Velocity.y<0 &&directionVector.y <0 ) {
                playerController.Velocity = new Vector2(-xBounceForce,yBounceForce);
            }
            else
            {
                
                playerController.Velocity = new Vector2(xBounceForce * Mathf.Sign(directionVector.x), 
                    yBounceForce);
            }
        }
       
    }
         
    private void Start() {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
}
