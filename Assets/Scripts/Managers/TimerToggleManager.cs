using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    /// <summary>
    /// Manager to manage the timer toggle setting UI.
    /// </summary>
    public class TimerToggleManager : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;

        private void Awake()
        {
            TimerManager timerManager = FindObjectOfType<TimerManager>();

            if (timerManager != null)
            {
                // frankly this could be set in the editor
                toggle.onValueChanged.AddListener(timerManager.OnToggle);
                timerManager.OnToggle(toggle.isOn);
            }
        }
    }
}
