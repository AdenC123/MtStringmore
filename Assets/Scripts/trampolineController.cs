using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trampolineController : MonoBehaviour
{
    #region Serialized Private Fields

    [Header("Bouncing")] 
    [SerializeField] private float yBounceForce;
    [SerializeField] private float xBounceForce;

    #endregion

    private PlayerController playerController;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
             Debug.Log("Player Entered Trampoline");
            // //playerController.Velocity = new Vector2(xBounceForce*Mathf.Sign(playerController.Velocity.x),yBounceForce);
            // playerController.Velocity = new Vector2(
            //     xBounceForce * playerController.Direction,  // Apply horizontal force
            //     yBounceForce  // Apply vertical bounce force (upwards)
            // );
            //collision.rigidbody.AddForce(yBounceForce * transform.up, ForceMode2D.Impulse); 
            //playerController.Velocity = new Vector2(10, 10);
            playerController.Velocity = handleBounce();
        }
        
    }
    void Start() {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    public Vector2 handleBounce()
    {
        return new Vector2(xBounceForce* playerController.Direction, yBounceForce);
    }
}
