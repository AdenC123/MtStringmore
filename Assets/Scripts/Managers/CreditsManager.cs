using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;

namespace Managers
{
    public class CreditsManager : MonoBehaviour
    {
        private void Awake()
        {
            PauseMenu.IsPauseDisabled = true;
        }
        
        private void Update()
        {
            if (InputUtil.StartJumpOrTouch()) LoadMainMenu();
        }

        private void OnDestroy()
        {
            PauseMenu.IsPauseDisabled = false;
        }

        private void LoadMainMenu()
        {
            SceneListManager.Instance.LoadMainMenu();
        }
    }
}
