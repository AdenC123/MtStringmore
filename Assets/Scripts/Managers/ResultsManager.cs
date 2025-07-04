using System.Collections.Generic;
using Interactables;
using Save;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class ResultsManager : MonoBehaviour
    {
        [SerializeField] private Checkpoint finalCheckpoint;
        
        [Header("Results Page")]
        [SerializeField] private GameObject resultsPane;
        
        [SerializeField] private TextMeshProUGUI levelHeaderText;

        [SerializeField] private TextMeshProUGUI collectableResultsText, deathsText;
        
        private int maxCount;
        
        private SaveDataManager _saveDataManager;
        private GameManager _gameManager;
        
        public static bool isResultsPageOpen = false;
        
        private void Start()
        {
            _saveDataManager = FindObjectOfType<SaveDataManager>();
            _gameManager = GameManager.Instance;
            maxCount =_gameManager.MaxCollectablesCount;
            levelHeaderText.text = "Level " + SceneManager.GetActiveScene().buildIndex / 2 + " Complete!";
            resultsPane.SetActive(false);
        }

        private void OnEnable()
        {
            if (finalCheckpoint != null)
                finalCheckpoint.OnCheckpointHit += HandleFinalCheckpointHit;
        }

        private void OnDisable()
        {
            if (finalCheckpoint != null)
                finalCheckpoint.OnCheckpointHit -= HandleFinalCheckpointHit;
        }

        private void HandleFinalCheckpointHit()
        {
            FindObjectOfType<LastCheckpoint>()?.UpdateLevelAccess();
            UpdateCollectableCount();
            UpdateDeathsCount();
            SaveGame();
            EndLevel();
        }

        private void SaveGame()
        {
            _gameManager.LevelCompleted();
        }

        private void UpdateCollectableCount()
        {
            int collectedCount = _gameManager.NumCollectablesCollected;
            collectableResultsText.text = collectedCount + " / " + maxCount;
        }
        
        private void UpdateDeathsCount()
        {
            int deaths = _gameManager.thisLevelDeaths;
            deathsText.text = deaths.ToString();
        }

        private void EndLevel()
        {
            Time.timeScale = 0f;
            resultsPane.SetActive(true); 
            isResultsPageOpen = true;
            _saveDataManager.SaveFile();
        }

        public void RestartLevel() 
        {
            Time.timeScale = 1f;
            _gameManager.ResetCandyCollected();
            isResultsPageOpen = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        public void LoadMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }

        public void LoadNextLevel()
        {
            resultsPane.SetActive(false);
            isResultsPageOpen = false;
            Time.timeScale = 1f;
            _gameManager.ResetCandyCollected();
            finalCheckpoint.StartConversation();
        }
    }
}
