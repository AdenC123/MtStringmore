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

        [SerializeField] private TextMeshProUGUI collectableResultsText;
        
        private int maxCount;

        private int collectedCount;
        
        private SaveDataManager _saveDataManager;
        private GameManager _gameManager;
        
        public static bool isResultsPageOpen = false;
        
        // <summary>
        // Access to cutscene names
        // <summary>
        [SerializeField] private List<string> cutsceneList;
        
        private void Start()
        {
            levelHeaderText.text = "Level " + SceneManager.GetActiveScene().buildIndex / 2 + " Complete!";
            resultsPane.SetActive(false);
            _saveDataManager = FindObjectOfType<SaveDataManager>();
            _gameManager = GameManager.Instance;
            
        }
        
        private void OnEnable()
        {
            finalCheckpoint.OnCheckpointHit += HandleFinalCheckpointHit;
        }
        
        private void OnDisable()
        {
            finalCheckpoint.OnCheckpointHit -= HandleFinalCheckpointHit;
        }

        private void HandleFinalCheckpointHit()
        {
            List<string> cl = _gameManager.cutsceneList;
            string sceneName = SceneManager.GetActiveScene().name;
            
            //check if we are in a level or cutscene
            if (cl.Contains(sceneName)) 
                FindObjectOfType<LastCheckpoint>()?.UpdateLevelAccess();
            UpdateCollectableCount();
            EndLevel();
        }

        private void UpdateCollectableCount()
        {
            maxCount = _gameManager.MaxCollectablesCount;
            collectedCount = _gameManager.NumCollectablesCollected;
            collectableResultsText.text = collectedCount + " / " + maxCount;
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
            GameManager.Instance.ResetCandyCollected();
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
