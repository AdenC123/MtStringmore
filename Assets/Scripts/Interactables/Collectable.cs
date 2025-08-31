using Managers;
using Player;
using UnityEngine;
using Util;

namespace Interactables
{
    /// <summary>
    /// This object is collected upon collision with the Player, incrementing the total collectable count.
    /// </summary>
    [RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
    public class Collectable : MonoBehaviour
    {
        [SerializeField] private Sprite[] possibleSprites;
        [SerializeField] private bool disableWithInteractables;
        
        private SpriteRenderer _spriteRenderer;
        private Collider2D _collider;
        private GameManager _gameManager;
        private bool _isCollected;
        
        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _gameManager = GameManager.Instance;
            _isCollected = false;
            _gameManager.Reset += OnReset;
        }
        
        private void Start()
        {
            if (disableWithInteractables)
            {
                OnInteractablesEnabledChanged();
                GameManager.Instance.OnInteractablesEnabledChanged += OnInteractablesEnabledChanged;
            }
        }

        private void OnDestroy()
        {
            _gameManager.Reset -= OnReset;
            if (disableWithInteractables)
            {
                GameManager.Instance.OnInteractablesEnabledChanged -= OnInteractablesEnabledChanged;
            }
        }

        private void OnReset()
        {
            if (_isCollected) GreyOut();
        }

        private void Collect()
        {
            // TODO: play a visual/particle effect before destroying
            if (!_isCollected) GameManager.Instance.CollectCollectable(this);
            SoundManager.Instance.PlayCollectableComboSound();
            _isCollected = true;
            _spriteRenderer.enabled = false;
            _collider.enabled = false;
        }
        
        private void GreyOut()
        {
            _spriteRenderer.enabled = true;
            _spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
            _collider.enabled = true;
            Debug.Log("Collectable Greyed Out");
        }

        private void OnValidate()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Collect();
            }
        }
        
        private void OnInteractablesEnabledChanged()
        {
            _spriteRenderer.enabled = GameManager.Instance.AreInteractablesEnabled;
            _collider.enabled = GameManager.Instance.AreInteractablesEnabled;
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
