using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused;
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    private DialogueRunner _dialogueRunner;

    private void Start()
    {
        _dialogueRunner = FindObjectOfType<DialogueRunner>();
        Resume();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        if (!_dialogueRunner.IsDialogueRunning) Time.timeScale = 1f;
        GameIsPaused = false;

        // Unmute audio using SoundManager
        SoundManager.Instance.SetMute(false);
    }

    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;

        // Mute audio using SoundManager
        SoundManager.Instance.SetMute(true);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}