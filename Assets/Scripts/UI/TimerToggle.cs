using Managers;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Script attached to the in-game timer toggle button.
    /// </summary>
    /// <remarks>
    /// Exists because the Manager keeps deleting itself.
    /// </remarks>
    public class TimerToggle : MonoBehaviour
    {
        /// <summary>
        /// Called by the In-game timer toggle.
        /// </summary>
        /// <param name="newValue">New timer toggle value</param>
        public void OnToggle(bool newValue)
        {
            TimerManager.Instance.OnToggle(newValue);
        }
    }
}
