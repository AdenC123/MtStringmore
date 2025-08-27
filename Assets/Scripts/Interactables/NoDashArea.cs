using Managers;
using UnityEngine;

namespace Interactables
{
    /// <summary>
    /// Disallows dashing within the collider area.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class NoDashArea : MonoBehaviour
    {
        [SerializeField] private bool disableWithInteractables;

        private Collider2D _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }

        private void Start()
        {
            if (disableWithInteractables)
            {
                _collider.enabled = GameManager.Instance.AreInteractablesEnabled;
                GameManager.Instance.OnInteractablesEnabledChanged += OnInteractablesEnabledChanged;
            }
        }

        private void OnDestroy()
        {
            if (disableWithInteractables)
            {
                GameManager.Instance.OnInteractablesEnabledChanged -= OnInteractablesEnabledChanged;
            }
        }

        /// <summary>
        /// Called when the interactables are enabled/disabled.
        /// </summary>
        private void OnInteractablesEnabledChanged()
        {
            _collider.enabled = GameManager.Instance.AreInteractablesEnabled;
        }
    }
}
