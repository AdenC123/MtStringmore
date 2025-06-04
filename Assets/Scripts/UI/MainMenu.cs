using Save;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Main menu canvas logic.
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private string startingScene;
        [SerializeField] private TextMeshProUGUI versionNumber;
        [SerializeField] private string levelSelectScene;
        [SerializeField] private Button loadGameButton;

        private SaveDataManager _saveDataManager;
        
        private void Start()
        {
            _saveDataManager = FindObjectOfType<SaveData Manager>();
            if (_saveDataManager == null)
            {
                Debug.LogError("SaveDataManager not found in scene!");
            }
        }

        private void Awake()
        {
            versionNumber.text = Application.version;
#if UNITY_WEBGL
            Debug.LogWarning("Saving and loading isn't supported on WebGL yet.");
            Destroy(loadGameButton.gameObject);
#endif
        }

        public void PlayGame()
        {
            _saveDataManager?.CreateNewSave(startingScene);
            SceneManager.LoadScene(startingScene);
        }
        public void LevelSelect()
        {
            SceneManager.LoadScene(levelSelectScene);
        }

        public void QuitGame()
        {
            Debug.Log("Quit!");
            Application.Quit();
        }
    }
}
