using System;
using System.Collections.Generic;
using System.Diagnostics;
using Managers;
using Player;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

namespace Knitby
{
    /// <summary>
    ///     Updates Knitby's position to follow the player's path
    /// </summary>
    public class KnitbyController : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private GameObject deathSmoke;

        [Header("Follow Settings")] [SerializeField]
        private float timeOffset = 0.1f;

        [SerializeField] private float idleThreshold = 0.07f;
        [SerializeField] private int granularity = 10;
        [SerializeField] private float interpolationSpeed = 20;
        
        [FormerlySerializedAs("shoulderOffset")]
        [Header("Attach Settings")] 
        [SerializeField] private Vector2 attachOffset = new Vector2(-1.0f, 1.5f);
        [SerializeField] private float attachLerpSpeed = 15f;
        [SerializeField] private bool _isPlayerHanging;

        [Header("Collisions")] [SerializeField]
        private LayerMask collisionLayer;

        [SerializeField] private float collisionDistance;

        public event Action<bool> SetIdle;

        public event Action<bool> SetWait;
        
        /// <summary>
        ///     Fires when direction updates.
        ///     Parameters: x and y, distances from position in previous frame
        /// </summary>
        public event Action<float, float> DirectionUpdated;

        /// <summary>
        ///     Fires when Knitby becomes grounded or leaves the ground.
        ///     Parameters:
        ///     bool: false if leaving the ground, true if becoming grounded
        ///     float: player's Y velocity
        /// </summary>
        public event Action<bool> GroundedChanged;

        /// <summary>
        ///     Fires continuously; true when currently in swing, false otherwise
        /// </summary>
        public event Action<bool> Swing;

        /// <summary>
        ///     Fires when player is dead
        /// Fires continuously; true when player can dash, false otherwise
        /// </summary>
        public event Action<bool> CanDash;

        /// <summary>
        ///     Fires when player is dead
        /// </summary>
        public event Action PlayerDeath;

        /// <summary>
        ///     Fires when Knitby hits a wall or leaves a wall.
        ///     False if leaving the wall, true if hitting a wall
        /// </summary>
        public event Action<bool> WallHitChanged;

        private readonly Queue<Vector3> _path = new();
        private CapsuleCollider2D _col;
        private Vector3 _currentPathPosition;
        private bool _grounded;
        private LineRenderer _lineRenderer;

        private GameObject _player;
        private PlayerController _playerController;
        private float _queueTimer;
        private bool _wallHit;
        private bool _isSwinging;

        private void Start()
        {
            _col = GetComponent<CapsuleCollider2D>();
            _player = GameObject.FindGameObjectWithTag("Player");
            _lineRenderer = _player.GetComponentInChildren<LineRenderer>();
            _playerController = _player.GetComponent<PlayerController>();
            _playerController.Death += PlayerDeath;
        }

        private void Update()
        {
            if (!_player) return;
            
            float xFlip = _playerController.Direction;
            
            if (_isPlayerHanging && !_isSwinging)
            {
                Vector3 targetPos = _player.transform.position + new Vector3(attachOffset.x * xFlip, attachOffset.y, 0f);
                transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * attachLerpSpeed); 
                DirectionUpdated?.Invoke(targetPos.x - transform.position.x, targetPos.y - transform.position.y);
                SetWait?.Invoke(_isPlayerHanging && !_isSwinging);
            }
            else
            {
                SetWait?.Invoke(_isPlayerHanging && !_isSwinging);
                if (_currentPathPosition == Vector3.zero) return;

                if (!_isSwinging)
                    SetIdle?.Invoke(Vector3.Distance(transform.position, _currentPathPosition) <= idleThreshold);

                Vector3 direction = _currentPathPosition - transform.position;

                DirectionUpdated?.Invoke(direction.x, direction.y);
                transform.position += direction * (Time.deltaTime * interpolationSpeed);
            }
        }

        private void FixedUpdate()
        {
            // granularity controls how many path positions can be stored in the queue before it is dequeued
            // this controls how far back we want this game object to lag behind the objectToFollow
            if (_player is null)
                return;
            _queueTimer -= Time.fixedDeltaTime;

            if (_queueTimer > 0) return;
            _queueTimer = timeOffset / granularity;
            
            if (_path.Count == granularity)
                _currentPathPosition = _path.Dequeue();
            _path.Enqueue(_player.transform.position);
            
            bool groundHit = CapsuleCastCollision(Vector2.down, collisionDistance);

            if (_grounded != groundHit)
            {
                _grounded = groundHit;
                GroundedChanged?.Invoke(groundHit);
            }

            bool wallHit = CapsuleCastCollision(Vector2.right, collisionDistance) ||
                           CapsuleCastCollision(Vector2.left, collisionDistance);
            if (_wallHit != wallHit)
            {
                _wallHit = wallHit;
                WallHitChanged?.Invoke(wallHit);
            }

            _isSwinging = _lineRenderer.isVisible;
            Swing?.Invoke(_isSwinging);

            CanDash?.Invoke(_playerController.CanDash);
        }

        private void OnEnable()
        {
            GameManager.Instance.Reset += OnReset;
            
            if (_player == null) _player = GameObject.FindWithTag("Player");
            
            PlayerController playerController = _player.GetComponent<PlayerController>();
            if (playerController) playerController.HangChanged += OnHangChanged;
        }

        private void OnDisable()
        {
            GameManager.Instance.Reset -= OnReset;
            if (!_player) return;
            PlayerController playerController = _player.GetComponent<PlayerController>();
            if (playerController) playerController.HangChanged += OnHangChanged;
            playerController.Death -= PlayerDeath;
        }

        private void OnHangChanged(bool isHanging, bool facingLeft)
        {
            _isPlayerHanging = isHanging;
        }
        
        private RaycastHit2D CapsuleCastCollision(Vector2 dir, float distance)
        {
            return Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0,
                dir, distance, collisionLayer);
        }

        /// <summary>
        ///     On reset, clear follow path and respawn at checkpoint position
        /// </summary>
        private void OnReset()
        {
            _path.Clear();
            Vector2 checkpointPos = GameManager.Instance.CheckPointPos;
            Vector3 spawnPos = new(checkpointPos.x, checkpointPos.y, transform.position.z);
            _currentPathPosition = spawnPos;
            transform.position = spawnPos;
        }
    }
}
