using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Balloon : MonoBehaviour
{
    [SerializeField] public float floatForce = 5f;
    [SerializeField] public bool isFloating = false;
	[SerializeField] public bool shouldRespawn = false;
    [SerializeField] private float maxHeight = 20f; //height balloon can go with player
    [SerializeField] private float  maxRespawnHeight = 30f; //height balloon can go before respawning
    [SerializeField] private string colliderTag = "Player";
    [SerializeField] private LayerMask collisionLayer;
	[SerializeField] private Vector3 respawnCoord = <-40, -6, 0>;

    public float groundDistance;

    public bool playerAttached = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == colliderTag)
        {
            isFloating = true;
            playerAttached = true;
        }
    }
    
    void Update()
    {
        HandleFloat();
        HandleDistanceToGround();
		HandleRespawn();
    }

    void HandleFloat()
    {
        if (playerAttached)
        {
            BalloonFloat(maxHeight);
        }
        else
        {
            BalloonFloat(maxRespawnHeight);
        }
    }

    void BalloonFloat(float Height) //maxHeight when player attached, maxRespawnHeight when player not attached
    {
        if (isFloating && transform.position.y <= Height)
        {
            transform.position += Vector3.up * (floatForce * Time.deltaTime);
        }
        else
        {
            isFloating = false;
        }
    }

    void HandleDistanceToGround()
    {
        Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y - 0.5f);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, collisionLayer);

        Debug.DrawRay(rayOrigin, Vector2.down * 10f, Color.red);

        if (hit.collider != null)
        {
            groundDistance = hit.distance;
            Debug.Log("Distance to ground: " + groundDistance);
        }
        else
        {
            Debug.Log("No hit detected");
        }
    }

    void HandleRespawn()
    {
        if (transform.position.y >= maxRespawnHeight)
        {
            shouldRespawn = true;
            isFloating = false;
        }
        
        if (shouldRespawn)
        {
            transform.position = respawnCoord;
            shouldRespawn = false;
        }
    }
}
