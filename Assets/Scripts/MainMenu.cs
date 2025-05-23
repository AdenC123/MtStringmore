using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string startingScene;
    [SerializeField] private string levelSelectScene;

    public void PlayGame()
    {
        SceneManager.LoadScene(startingScene);
    }
    
    public void LevelSelect() {
        SceneManager.LoadScene(levelSelectScene);
    }
    
    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }
}
