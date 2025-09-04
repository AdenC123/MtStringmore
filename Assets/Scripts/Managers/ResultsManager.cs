using System.Linq;
using Save;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class ResultsManager : MonoBehaviour
    {
        public static bool isResultsPageOpen;

        [Header("Results Page")]
        [SerializeField] private ResultsWindow resultsWindow;
        [SerializeField] private GameObject menuButton;
        [SerializeField] private string[] disableMainMenuInScenes;
        
        private LastCheckpoint _lastCheckpoint;

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (_lastCheckpoint)
            {
                _lastCheckpoint.AttachedCheckpoint.onCheckpointReached.RemoveListener(HandleFinalCheckpointHit);
                _lastCheckpoint = null;
            }

            _lastCheckpoint = FindAnyObjectByType<LastCheckpoint>();
            if (_lastCheckpoint)
            {
                _lastCheckpoint.AttachedCheckpoint.onCheckpointReached.AddListener(HandleFinalCheckpointHit);
            }

            bool inDisableMainMenuScene = disableMainMenuInScenes.Contains(SceneManager.GetActiveScene().name);
            menuButton.SetActive(!inDisableMainMenuScene);
            
            resultsWindow.gameObject.SetActive(false);
            isResultsPageOpen = false;
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (_lastCheckpoint) _lastCheckpoint.AttachedCheckpoint.onCheckpointReached.RemoveListener(HandleFinalCheckpointHit);
        }

        private void HandleFinalCheckpointHit()
        {
            Time.timeScale = 0;
            if (_lastCheckpoint) _lastCheckpoint.UpdateLevelAccess();
            GameManager.Instance.SaveGame();
            resultsWindow.gameObject.SetActive(true);
            resultsWindow.UpdateDisplay();
            isResultsPageOpen = true;
            FindAnyObjectByType<OnSceneButtons>().SetRestartButtonState(false);
            PauseMenu.IsPauseDisabled = true;
        }

        public void RestartLevel() 
        {
            Time.timeScale = 1f;
            GameManager.Instance.ResetCandyCollected();
            isResultsPageOpen = false;
            PauseMenu.IsPauseDisabled = false;
            SceneListManager.Instance.RestartLevel();
        }
        
        public void LoadMainMenu()
        {
            Time.timeScale = 1f;
            PauseMenu.IsPauseDisabled = false;
            SceneListManager.Instance.LoadMainMenu();
        }

        public void LoadNextLevel()
        {
            resultsWindow.gameObject.SetActive(false);
            isResultsPageOpen = false;
            Time.timeScale = 1f;
            PauseMenu.IsPauseDisabled = false;
            GameManager.Instance.ResetCandyCollected();
            _lastCheckpoint.AttachedCheckpoint.StartConversation();
        }
    }
}
