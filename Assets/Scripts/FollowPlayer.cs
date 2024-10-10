using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private GameObject objectToFollow;
    [SerializeField] private float timeOffset = 0.1f;
    [SerializeField] private int granularity = 10;

    [SerializeField] private float interpolationSpeed = 20;

    private Queue<Vector3> path = new Queue<Vector3>();
    private float queueTimer;

    private Vector3 currentPathPosition;

    // Update is called once per frame
    void Update()
    {
        // If the objectToFollow has moved, this moves this GO towards the new position with speed = interpolationSpeed
        if (currentPathPosition != Vector3.zero)
            transform.position += (currentPathPosition - transform.position) * Time.deltaTime * interpolationSpeed;

        // sets Knitby's sprite renderer to be visible depending on whether the yarn string is on screen or not
        this.gameObject.GetComponent<SpriteRenderer>().enabled = !objectToFollow.GetComponentInChildren<LineRenderer>().isVisible;
    }

    void FixedUpdate()
    {
        // granularity controls how many path positions can be stored in the queue before it is dequeued
        // this controls how far back we want this GO to lag behind the objectToFollow
        if (objectToFollow == null)
            return;
        queueTimer -= Time.fixedDeltaTime;

        if (queueTimer <= 0)
        {
            queueTimer = timeOffset / granularity;
            if (path.Count == granularity)
                currentPathPosition = path.Dequeue();
            path.Enqueue(objectToFollow.transform.position);
        }
    }
}
