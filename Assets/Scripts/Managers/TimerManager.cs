using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public class TimerManager : MonoBehaviour
    {
        private static TimerManager _instance;
        public static TimerManager Instance
        {
            get
            {
                if (!_instance)
                    _instance = FindObjectOfType<TimerManager>();

                return _instance;
            }
        }

        [SerializeField] private TextMeshProUGUI inGameTimerText;
        [SerializeField] private GameObject resultsWindow;
        [SerializeField] private Toggle timerToggle;
        
        public static float ElapsedLevelTime { get; set; }
        public static string ElapsedLevelTimeString {get; private set;}

        // public bool IsResultsWindowActive => resultsWindow.activeSelf;
        // public bool IsTimerShown => inGameTimerText.enabled;

        private void Awake()
        {
            if (Instance != this) Destroy(gameObject);
            //only reset time when in a level, not in a cutscene
            if (!GameManager.Instance.IsInCutsceneOrMainMenu())
            {
                ElapsedLevelTime = 0;
            }
            inGameTimerText.enabled = false;
        }
        
        private void Start()
        {
            int savedSpeedToggle = PlayerPrefs.GetInt("SpeedTime",0);
            bool toggle = savedSpeedToggle == 1;
            ToggleTimer(toggle);
        }
        
        private void Update()
        { 
            if (resultsWindow.activeSelf || GameManager.Instance.IsInCutsceneOrMainMenu())
            {
                inGameTimerText.enabled = false;
                return;
            }
            
            inGameTimerText.enabled = timerToggle.isOn;

            ElapsedLevelTime += Time.deltaTime;
            
            TimeSpan timeSpan = TimeSpan.FromSeconds(ElapsedLevelTime);
            ElapsedLevelTimeString = timeSpan.ToString(@"mm\:ss\:ff");

            inGameTimerText.text = ElapsedLevelTimeString;
        }

        public void OnToggle()
        {
            inGameTimerText.enabled = timerToggle.isOn;
            ToggleTimer(timerToggle.isOn);
        }

        public void ToggleTimer(bool toggle)
        {
            timerToggle.isOn = toggle;
            int value = toggle? 1:0;
            PlayerPrefs.SetInt("SpeedTime", value);
            PlayerPrefs.Save();
        }
    }
}
