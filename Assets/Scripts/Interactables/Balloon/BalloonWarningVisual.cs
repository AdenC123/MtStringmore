using System.Collections;
using UnityEngine;

namespace Interactables.Balloon
{
    /// <summary>
    /// Warning visual to increase and decrease the size of the visual when enabled.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class BalloonWarningVisual : MonoBehaviour
    {
        [SerializeField, Tooltip("New size of the object when warning is playing")] private float warningSize = 1.2f;
        [SerializeField, Tooltip("Time with new size (sec)"), Min(0)] private float timeInWarning = 0.3f;

        private void OnEnable()
        {
            StartCoroutine(WarningCoroutine());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            transform.localScale = Vector3.one;
        }

        /// <summary>
        /// Coroutine to adjust the size of this object every <see cref="timeInWarning"/> seconds.
        /// </summary>
        /// <returns>Coroutine</returns>
        private IEnumerator WarningCoroutine()
        {
            WaitForSeconds wait = new(timeInWarning);
            while (enabled)
            {
                transform.localScale = Vector3.one * warningSize;
                yield return wait;
                transform.localScale = Vector3.one;
                yield return wait;
            }
        }
    }
}
