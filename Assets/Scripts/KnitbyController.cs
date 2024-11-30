using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Updates Knitby's position to follow the player's path
/// </summary>
public class KnitbyController : MonoBehaviour
{
    [SerializeField] private GameObject objectToFollow;
    [SerializeField] private float timeOffset = 0.1f;
    [SerializeField] private int granularity = 10;
    [SerializeField] private float interpolationSpeed = 20;
    
    public Vector3 direction;
    
    private Vector3 _currentPathPosition;
    private readonly Queue<Vector3> _path = new();
    private float _queueTimer;

    private void Update()
    {
        if (_currentPathPosition != Vector3.zero)
        {
            direction = _currentPathPosition - transform.position;
            transform.position += direction * (Time.deltaTime * interpolationSpeed);
        }
    }

    private void FixedUpdate()
    {
        // granularity controls how many path positions can be stored in the queue before it is dequeued
        // this controls how far back we want this game object to lag behind the objectToFollow
        if (objectToFollow is null)
            return;
        _queueTimer -= Time.fixedDeltaTime;

        if (!(_queueTimer <= 0)) return;
        _queueTimer = timeOffset / granularity;
        if (_path.Count == granularity)
            _currentPathPosition = _path.Dequeue();
        _path.Enqueue(objectToFollow.transform.position);
    }
}