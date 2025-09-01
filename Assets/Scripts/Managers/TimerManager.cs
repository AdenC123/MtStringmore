using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Yarn.Unity;

namespace Managers
{
    /// <summary>
    /// Manager of the in game timer.
    /// </summary>
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

        /// <summary>
        /// Elapsed level time, seconds.
        /// </summary>
        public float ElapsedLevelTime { get; private set; }

        /// <summary>
        /// Human readable string for the elapsed level time.
        /// </summary>
        public string ElapsedLevelTimeString => TimeSpan.FromSeconds(ElapsedLevelTime).ToString(@"mm\:ss\:ff");

        /// <summary>
        /// Whether the timer should be hidden even if the setting is enabled.
        /// </summary>
        private bool ShouldForceHideTimer => !_isNotForceDisabled || resultsWindow.activeSelf ||
                                             SceneListManager.Instance.InCutscene ||
                                             SceneListManager.Instance.InMainMenu;
        
        private bool _isNotForceDisabled = true;

        private void Awake()
        {
            if (Instance != this) Destroy(gameObject);

            UpdateFromPrefs();
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void Update()
        {
            if (ShouldForceHideTimer) return;

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
            _isNotForceDisabled = true;
            //only reset time when in a level, not in a cutscene
            if (!SceneListManager.Instance.InCutscene)
            {
                ElapsedLevelTime = 0;
            }
            UpdateFromPrefs();
        }

        /// <summary>
        /// Updates the timer toggle/text depending on the saved PlayerPref.
        /// </summary>
        private void UpdateFromPrefs()
        {
            int savedSpeedToggle = PlayerPrefs.GetInt("SpeedTime", 0);
            bool userShowToggle = savedSpeedToggle == 1;
            timerToggle.isOn = userShowToggle;
            inGameTimerText.enabled = !ShouldForceHideTimer && userShowToggle;
        }

        /// <summary>
        /// Sets the timer state.
        /// </summary>
        /// <param name="value">True if the timer should be enabled</param>
        [YarnCommand("timer_state")]
        public void SetTimerState(bool value)
        {
            _isNotForceDisabled = value;
            inGameTimerText.enabled = !ShouldForceHideTimer && timerToggle.isOn;
        }

        /// <summary>
        /// Called by the toggle - updates the PlayerPref and the in game timer.
        /// </summary>
        public void OnToggle()
        {
            inGameTimerText.enabled = timerToggle.isOn && !ShouldForceHideTimer;
            int value = timerToggle.isOn ? 1 : 0;
            PlayerPrefs.SetInt("SpeedTime", value);
            PlayerPrefs.Save();
        }
    }
}
