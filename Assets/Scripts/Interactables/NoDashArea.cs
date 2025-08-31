using Managers;
using Player;
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
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out PlayerController player))
            {
                player.EnterNoDashArea();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent(out PlayerController player))
            {
                player.ExitNoDashArea();
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
