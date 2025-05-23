using System;
using Save;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultsManager : MonoBehaviour
{
    [SerializeField] private Checkpoint finalCheckpoint;
    
    [Header("Results Page")]
    [Tooltip("If checked, the level has ended and the results page should be displayed")]
    [SerializeField] private bool displayResults;
    
    [SerializeField] private GameObject resultsPane;

    [SerializeField] private GameObject collectableResults;

    [SerializeField] private int maxCount;
    
    private SaveDataManager _saveDataManager;
    
    private void Start()
    {
        resultsPane.GetComponentInChildren<TextMeshProUGUI>().text = "Level " + SceneManager.GetActiveScene().buildIndex / 2 + " Complete!";
        resultsPane.SetActive(false);
        _saveDataManager = FindObjectOfType<SaveDataManager>();
    }

    private void Update()
    {
        if (!finalCheckpoint.hasBeenHit) return;
        if (Input.GetKeyDown(KeyCode.R)) return;
        UpdateCollectableCount();
        EndLevel();
    }

    private void UpdateCollectableCount()
    {
        var collectedCount = GameManager.Instance.NumCollected;
        collectableResults.GetComponentInChildren<TextMeshProUGUI>().text = collectedCount + " / " + maxCount;
    }

    private void EndLevel()
    {
        Time.timeScale = 0f;
        resultsPane.SetActive(true);
        _saveDataManager.SaveFile();
    }

    public void RestartLevel() {
        Time.timeScale = 1f;
        GameManager.Instance.NumCollected = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        GameManager.Instance.NumCollected = 0;
        resultsPane.SetActive(false);
        finalCheckpoint.StartConversation();
    }
}

