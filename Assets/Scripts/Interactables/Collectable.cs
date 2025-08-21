using System;
using Managers;
using Player;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

namespace Interactables
{
    /// <summary>
    /// This object is collected upon collision with the Player, incrementing the total collectable count.
    /// </summary>
    [RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
    public class Collectable : MonoBehaviour
    {
        [SerializeField] private Sprite[] possibleSprites;
        
        private SpriteRenderer _spriteRenderer;
        private Collider2D _collider;
        private PlayerController player;
        private bool _isCollected;
        
        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            player = FindObjectOfType<PlayerController>();
            _isCollected = false;
        }

        private void OnEnable()
        {
            player.Death += OnPlayerDeath;
        }

        private void OnPlayerDeath()
        {
            if (_isCollected) GreyOut();
        }

        private void Collect()
        {
            // TODO: play a visual/particle effect before destroying
            GameManager.Instance.CollectCollectable(this);
            SoundManager.Instance.PlayCollectableComboSound();
            _isCollected = true;
            _spriteRenderer.enabled = false;
            _collider.enabled = false;
        }
        
        private void GreyOut()
        {
            _spriteRenderer.enabled = true;
            _spriteRenderer.color = Color.gray;
            _collider.enabled = false;
        }

        // /// <summary>
        // /// Resets the collectable to default
        // /// </summary>
        // public void ResetCollectable()
        // {
        //     _isCollected = false;
        //     _spriteRenderer.enabled = true;
        //     _spriteRenderer.color = Color.white;
        // }

        private void OnValidate()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !_isCollected)
            {
                Collect();
            }
        }

        /// <summary>
        /// Chooses a random sprite from the possible sprites of this collectable.
        /// Should only be run in editor.
        /// </summary>
        public void RandomizeSprite()
        {
            _spriteRenderer.sprite = RandomUtil.SelectRandom(possibleSprites);
        }

        /// <summary>
        /// Rotates this collectable's transform to a random angle.
        /// Should only be run in editor.
        /// </summary>
        public void RandomizeRotation()
        {
            transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 360));
        }
    }
}
