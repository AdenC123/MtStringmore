using System;
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
        
        private SaveDataManager _saveDataManager;

        public static bool isResultsPageOpen;
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (finalCheckpoint)
            {
                finalCheckpoint.OnCheckpointHit -= HandleFinalCheckpointHit;
                finalCheckpoint = null;
            }
            
            GameObject checkpointObj = GameObject.FindWithTag("FinalCheckpoint");
            if (checkpointObj)
            {
                finalCheckpoint = checkpointObj.GetComponent<Checkpoint>();
                if (finalCheckpoint != null) finalCheckpoint.OnCheckpointHit += HandleFinalCheckpointHit;
            }
            else finalCheckpoint = null;
            
            maxCount = FindObjectsOfType<Collectable>().Length;
            levelHeaderText.text = "Level " + SceneManager.GetActiveScene().buildIndex / 2 + " Complete!";
            resultsPane.SetActive(false);
            isResultsPageOpen = false;
            _saveDataManager = FindObjectOfType<SaveDataManager>();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (finalCheckpoint != null) finalCheckpoint.OnCheckpointHit -= HandleFinalCheckpointHit;
        }

        private void HandleFinalCheckpointHit()
        {
            FindObjectOfType<LastCheckpoint>()?.UpdateLevelAccess();
            UpdateCollectableCount();
            EndLevel();
        }

        private void Update()
        {
            if (resultsPane.activeSelf && Input.GetButtonDown("Debug Reset")) return;
        }

        private void UpdateCollectableCount()
        {
            int collectedCount = GameManager.Instance.NumCollectablesCollected;
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
            GameManager.Instance.ResetCandyCollected();
            finalCheckpoint.StartConversation();
        }
    }
}
