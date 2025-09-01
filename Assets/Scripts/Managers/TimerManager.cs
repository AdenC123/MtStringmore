using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Yarn.Unity;

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

        private bool _isEnabled = true;
        public float ElapsedLevelTime { get; private set; }
        public string ElapsedLevelTimeString => TimeSpan.FromSeconds(ElapsedLevelTime).ToString(@"mm\:ss\:ff");

        private void Awake()
        {
            if (Instance != this) Destroy(gameObject);

            inGameTimerText.enabled = false;
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void Start()
        {
            int savedSpeedToggle = PlayerPrefs.GetInt("SpeedTime", 0);
            bool toggle = savedSpeedToggle == 1;
            ToggleTimer(toggle);
        }

        private void Update()
        {
            if (!_isEnabled) return;
            if (resultsWindow.activeSelf || SceneListManager.Instance.InCutscene || SceneListManager.Instance.InMainMenu)
            {
                inGameTimerText.enabled = false;
                return;
            }

            inGameTimerText.enabled = timerToggle.isOn;

            ElapsedLevelTime += Time.deltaTime;
            // slight performance gain from not updating the text if not displayed
            if (inGameTimerText.enabled)
                inGameTimerText.text = ElapsedLevelTimeString;
        }

        /// <summary>
        /// Runs on scene load.
        /// </summary>
        /// <param name="newScene">New scene</param>
        /// <param name="mode">Scene load mode</param>
        private void OnSceneLoaded(Scene newScene, LoadSceneMode mode)
        {
            _isEnabled = true;
            //only reset time when in a level, not in a cutscene
            if (!SceneListManager.Instance.InCutscene)
            {
                ElapsedLevelTime = 0;
            }
        }

        [YarnCommand("timer_state")]
        public void SetTimerState(bool value)
        {
            _isEnabled = value;
            inGameTimerText.enabled = value;
        }

        public void OnToggle()
        {
            inGameTimerText.enabled = timerToggle.isOn;
            ToggleTimer(timerToggle.isOn);
        }

        public void ToggleTimer(bool toggle)
        {
            timerToggle.isOn = toggle;
            int value = toggle ? 1 : 0;
            PlayerPrefs.SetInt("SpeedTime", value);
            PlayerPrefs.Save();
        }
    }
}
