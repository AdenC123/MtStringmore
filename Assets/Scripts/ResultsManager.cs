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
    
    [Header("References")] [SerializeField] 
    private GameManager gameManager;
    
    private void Start()
    {
        resultsPane.GetComponentInChildren<TextMeshProUGUI>().text = "Level " + SceneManager.GetActiveScene().buildIndex / 2;
        resultsPane.SetActive(false);
    }

    private void Update()
    {
        if (!finalCheckpoint.hasBeenHit) return;
        EndLevel();
    }
    
    private void EndLevel()
    {
        Time.timeScale = 0f;
        resultsPane.SetActive(true);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        resultsPane.SetActive(false);
        finalCheckpoint.StartConversation();
    }
}

