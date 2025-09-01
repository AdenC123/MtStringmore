using System;
using TMPro;
using UnityEngine;
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

        private static bool _isEnabled = true;
        public static float ElapsedLevelTime { get; private set; }
        public static string ElapsedLevelTimeString => TimeSpan.FromSeconds(ElapsedLevelTime).ToString(@"mm\:ss\:ff");

        // public bool IsResultsWindowActive => resultsWindow.activeSelf;
        // public bool IsTimerShown => inGameTimerText.enabled;

        private void Awake()
        {
            if (Instance != this) Destroy(gameObject);
            //only reset time when in a level, not in a cutscene
            if (!SceneListManager.Instance.InCutscene)
            {
                ElapsedLevelTime = 0;
            }

            inGameTimerText.enabled = false;
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
