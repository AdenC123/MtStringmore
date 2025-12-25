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
        
        [Header("Attach Settings")]
        [SerializeField] private Vector2 attachOffset;
        [SerializeField] private float attachLerpSpeed = 70f;

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
        private Animator _animator;
        private float _queueTimer;
        private bool _wallHit;
        private bool _isPlayerHanging;
        private bool _isSwinging;
        private bool _isSwingingClockwise;
        private bool _hasSpun;
        
        private void Start()
        {
            _col = GetComponent<CapsuleCollider2D>();
            _animator = GetComponentInChildren<Animator>();
            
            _player = GameObject.FindGameObjectWithTag("Player");
            _playerController = _player.GetComponent<PlayerController>();
            _lineRenderer = _player.GetComponentInChildren<LineRenderer>();
            
            _playerController.HangChanged += OnHangChanged;
            _playerController.SwingChanged += OnSwingChanged;
            _playerController.SwingDifferentDirection += OnSwingDifferentDirection;
            _playerController.Death += PlayerDeath;
        }

        private void Update()
        {
            if (!_player) return;
            HandleGrounded();
            HandleSwing();
            HandleHang();
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
            
            CanDash?.Invoke(_playerController.CanDash);
        }
        
        /// <summary>
        ///     Handle states when on the ground
        /// </summary>
        private void HandleGrounded()
        {
            if (_isPlayerHanging || _isSwinging) return;
            
            SetIdle?.Invoke(Vector3.Distance(transform.position, _currentPathPosition) <= idleThreshold);
            if (_currentPathPosition == Vector3.zero) return;
            
            Vector3 direction = _currentPathPosition - transform.position;
            DirectionUpdated?.Invoke(direction.x, direction.y);
            transform.position += direction * (Time.deltaTime * interpolationSpeed);
        }
        
        /// <summary>
        ///     Handle hanging states except swinging
        /// </summary>
        private void HandleHang()
        {
            if (!_isPlayerHanging || _isSwinging) return;
            
            float dir = _playerController.Direction;
            HandleHangingPosition(dir);
        }
        
        /// <summary>
        ///     Handle logic when Knitby has spun and is now on player's back on the swing
        /// </summary>
        private void HandleSwing()
        {
            if (!_isSwinging) return;
            if (!_hasSpun)
            {
                _hasSpun = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
                return;
            }
            
            float dir = _isSwingingClockwise ? -1f : 1f;
            HandleHangingPosition(dir);
        }
        
        /// <summary>
        ///     When player is hanging, move Knitby to the right position + rotation
        /// </summary>
        private void HandleHangingPosition(float dir)
        {
            transform.rotation = _player.transform.rotation;
            Vector3 localOffset = new Vector3(attachOffset.x * dir, attachOffset.y, 0f);
            Vector3 rotatedOffset = _player.transform.TransformDirection(localOffset);
            Vector3 targetPos = _player.transform.position + rotatedOffset;
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * attachLerpSpeed);
        }

        private void OnEnable()
        {
            GameManager.Instance.Reset += OnReset;
            if (!_player || !_playerController) return;
            _playerController.HangChanged += OnHangChanged;
            _playerController.SwingChanged += OnSwingChanged;
            _playerController.Death += PlayerDeath;
        }

        private void OnDisable()
        {
            GameManager.Instance.Reset -= OnReset;
            if (!_player || !_playerController) return;
            _playerController.HangChanged -= OnHangChanged;
            _playerController.SwingChanged -= OnSwingChanged;
            _playerController.Death -= PlayerDeath;
        }

        private void OnHangChanged(bool isHanging, bool facingLeft)
        {
            SetWait?.Invoke(isHanging);
            _isPlayerHanging = isHanging;
            if (!isHanging)
                transform.rotation = Quaternion.identity;
        }
        
        private void OnSwingChanged(bool isSwinging)
        {
            _isSwinging = isSwinging;
            _hasSpun = false;
            Swing?.Invoke(_isSwinging);
        }
        
        private void OnSwingDifferentDirection(bool clockwise)
        {
            _isSwingingClockwise = clockwise;
            DirectionUpdated.Invoke(clockwise ? -1f : 1f, 0f);
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
            transform.rotation = Quaternion.identity;
        }
    }
}
