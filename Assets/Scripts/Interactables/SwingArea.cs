using Managers;
using UnityEngine;

namespace Interactables
{
    /// <summary>
    /// Represents the 'swing area' in a button.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class SwingArea : MonoBehaviour
    {
        [Min(0), Tooltip("Swing radius (local space)")] public float swingRadius;

        [SerializeField] private SpriteRenderer buttonSprite;
        
        /// <summary>
        /// Swings are enabled by default, but override with the disable sprite if GameManager flag is set
        /// </summary>
        [SerializeField] private Sprite disabledSprite;

        private Sprite _enabledSprite;
        private SpriteRenderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _enabledSprite = _renderer.sprite;
            GameManager.Instance.OnInteractablesEnabledChanged += UpdateSprite;
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnInteractablesEnabledChanged -= UpdateSprite;
        }

        /// <summary>
        /// Updates the sprite depending on whether interactables are enabled.
        /// </summary>
        private void UpdateSprite()
        {
            bool interactablesEnabled = GameManager.Instance.AreInteractablesEnabled;
            buttonSprite.enabled = interactablesEnabled;
            _renderer.sprite = interactablesEnabled ? _enabledSprite : disabledSprite;
        }
    }
}
