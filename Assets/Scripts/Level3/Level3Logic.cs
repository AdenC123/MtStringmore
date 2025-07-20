using Interactables;
using Managers;
using UnityEngine;
using UnityEngine.Events;

namespace Level3
{
    /// <summary>
    /// Custom level 3 logic.
    /// </summary>
    public class Level3Logic : MonoBehaviour
    {
        [SerializeField] private UnityEvent onSecondHalfReached;

        private Checkpoint[] _checkpoints;

        private void Awake()
        {
            _checkpoints = FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);
        }

        private void Start()
        {
            // there's no guarantee we grab the right instance in Awake so we use Start
            GameManager.Instance.AreInteractablesEnabled = false;
        }

        /// <summary>
        /// Called upon second half being reached - enables interactables, clears checkpoints and flips them.
        /// </summary>
        public void ReachSecondHalf()
        {
            GameManager.Instance.AreInteractablesEnabled = true;
            // seems checkpoint calls this before adding itself
            // makes our life easier
            GameManager.Instance.ClearCheckpointData();
            foreach (Checkpoint checkpoint in _checkpoints)
            {
                checkpoint.FlipCheckpoint();
            }
            onSecondHalfReached.Invoke();
        }
    }
}
