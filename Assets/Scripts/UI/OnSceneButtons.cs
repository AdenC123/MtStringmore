using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OnSceneButtons : MonoBehaviour
{
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    
    private void Awake()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
    }
    
    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }
    
    private void OnSceneChanged(Scene current, Scene next)
    {
        bool isMainMenu = next.name == mainMenuSceneName;
        // Hacky but just check if the string "cutscene" is in the name of the scene
        bool isCutscene = next.name.ToLower().Contains("cutscene");

        pauseButton.gameObject.SetActive(!isMainMenu);
        restartButton.gameObject.SetActive(!isMainMenu && !isCutscene);
        
        Debug.Log($"Scene changed to {next.name} → Pause: {!isMainMenu}, Restart: {!isMainMenu && !isCutscene}");
    }
}
