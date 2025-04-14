using Save;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Main menu canvas logic.
/// </summary>
public class MainMenu : MonoBehaviour
{
    [SerializeField] private string startingScene;

    private SaveDataManager _saveDataManager;

    private void Awake()
    {
        _saveDataManager = FindObjectOfType<SaveDataManager>();
    }

    public void PlayGame()
    {
        _saveDataManager?.CreateNewSave();
        SceneManager.LoadScene(startingScene);
    }

    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }
}
