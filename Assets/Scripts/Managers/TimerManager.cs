using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public class TimerManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI inGameTimerText;
        [SerializeField] private GameObject resultsWindow;
        [SerializeField] private Toggle timerToggle;
        
        public static float ElapsedLevelTime { get; set; }

        // public bool IsResultsWindowActive => resultsWindow.activeSelf;
        // public bool IsTimerShown => inGameTimerText.enabled;

        private void Awake()
        {
            inGameTimerText.enabled = false;
        }
        
        private void Start()
        {
            ElapsedLevelTime = 0;
        }
        
        private void Update()
        { 
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            
            if (resultsWindow.activeSelf || sceneIndex == 0 || sceneIndex % 2 != 0)
            {
                inGameTimerText.enabled = false;
                return;
            }
            
            inGameTimerText.enabled = timerToggle.isOn;

            ElapsedLevelTime += Time.deltaTime;
            
            TimeSpan timeSpan = TimeSpan.FromSeconds(ElapsedLevelTime);
            string text = timeSpan.ToString(@"mm\:ss\:ff");

            timerText.text = text;
            inGameTimerText.text = text;
        }

        public void OnToggle()
        {
            inGameTimerText.enabled = timerToggle.isOn;
        }
    }
}
