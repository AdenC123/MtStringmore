using System.Collections;
using TMPro;
using UnityEngine;

namespace Managers
{
    public class TimerManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI inGameTimerText;
        [SerializeField] private GameObject resultsWindow;
        
        private float _elapsedLevelTime;
        
        // public bool IsResultsWindowActive => resultsWindow.activeSelf;
        // public bool IsTimerShown => inGameTimerText.enabled;

        private void Awake()
        {
            inGameTimerText.enabled = false;
        }
        
        private void Start()
        {
            _elapsedLevelTime = 0;
        }
        
        private void Update()
        {
            bool previousState = inGameTimerText.enabled;
            
            if (resultsWindow.activeSelf)
            {
                inGameTimerText.enabled = false;
                return;
            }
            
            inGameTimerText.enabled = previousState;
            
            _elapsedLevelTime += Time.deltaTime;
            int minutes = (int) (_elapsedLevelTime / 60);
            int seconds = (int) (_elapsedLevelTime % 60);
            int milliseconds = (int) ((_elapsedLevelTime * 100) % 100);
            
            timerText.text = $"{minutes:00}:{seconds:00}:{milliseconds:00}";
            inGameTimerText.text = $"{minutes:00}:{seconds:00}:{milliseconds:00}";
        }

        public void OnToggle(bool isOn)
        {
            inGameTimerText.enabled = isOn;
        }
    }
}
