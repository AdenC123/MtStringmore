using Player;
using UnityEngine;

namespace Interactables.Balloon
{
    /// <summary>
    /// Trigger to tell the balloon the player is closeby.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class BalloonTrigger : MonoBehaviour
    {
        private Balloon _parent;
        private void Awake()
        {
            _parent = GetComponentInParent<Balloon>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player) _parent.OnPlayerEnterInflationZone();
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player) _parent.OnPlayerExitInflationZone();
        }
    }
}
