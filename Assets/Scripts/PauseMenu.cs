using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused;
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    private float _prevTimescale;

    private void Start()
    {
        _prevTimescale = Time.timeScale;
        Resume();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != mainMenuSceneName)
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
        Time.timeScale = _prevTimescale;
        GameIsPaused = false;

        // Unmute audio using SoundManager
        SoundManager.Instance.SetMute(false);
    }

    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        _prevTimescale = Time.timeScale;
        Time.timeScale = 0f;
        GameIsPaused = true;

        // Mute audio using SoundManager
        SoundManager.Instance.SetMute(true);
    }

    public void LoadMenu()
    {
        // reset any changes made by pausing
        Resume();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}