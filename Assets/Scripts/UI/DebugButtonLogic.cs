using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    /// <summary>
    /// Special debug button logic to enable all the scenes.
    /// </summary>
    public class DebugButtonLogic : MonoBehaviour
    {
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private UnityEvent eventOnTrigger;
        private int _debugButtonPressed;
        
        /// <summary>
        /// Debug button pressed.
        /// </summary>
        public void DebugButtonPressed()
        {
            _debugButtonPressed++;
            if (_debugButtonPressed > 10)
            {
                int childCount = buttonContainer.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    LevelSelectButton btn = buttonContainer.GetChild(i).GetComponent<LevelSelectButton>();
                    btn.Initialize(i + 1, true);
                }
                eventOnTrigger.Invoke();
            }
        }
    }
}
