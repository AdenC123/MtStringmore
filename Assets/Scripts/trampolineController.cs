using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the trampoline physics 
/// </summary>
public class trampolineController : MonoBehaviour
{
    #region Serialized Private Fields
    [Header("Bouncing")] 
    [SerializeField] private float yBounceForce;
    [SerializeField] private float xBounceForce;
    #endregion

    private PlayerController playerController;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerController.Velocity = handleBounce();
        }
        
    }
    private void Start() {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    public Vector2 handleBounce()
    {
        return new Vector2(xBounceForce * playerController.Direction, yBounceForce);
    }
}
