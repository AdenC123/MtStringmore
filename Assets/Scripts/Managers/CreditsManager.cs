using UnityEngine;
using UnityEngine.SceneManagement;
using Util;
using Yarn.Unity;

namespace Managers
{
    public class CreditsManager : MonoBehaviour
    {
        [SerializeField] private string mainMenuSceneName = "MainMenu";
        private static bool _canExit;

        void Start()
        {
            _canExit = false;
        }

        void Update()
        {
            if (InputUtil.StartJumpOrTouch() && _canExit) LoadMainMenu();
            
        }

        private void LoadMainMenu()
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        
        [YarnCommand("enable_exit")]
        public static void EnableExit()
        {
            _canExit = true;
        }
    }
}
