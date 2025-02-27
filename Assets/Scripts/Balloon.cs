using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Balloon : MonoBehaviour
{

    [SerializeField] public float floatForce = 5f;
    [SerializeField] public bool isFloating = false;
    [SerializeField] private float maxHeight = 20f;
    [SerializeField] private string colliderTag = "Player";
    [SerializeField] private LayerMask collisionLayer;

    public float groundDistance;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == colliderTag)
        {
            isFloating = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleFloat();
        HandleDistanceToGround();
    }

    void HandleFloat()
    {
        if (isFloating && transform.position.y <= maxHeight)
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
}
