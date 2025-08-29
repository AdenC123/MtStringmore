using UnityEngine;
using UnityEngine.SceneManagement;
using Util;

namespace Managers
{
    public class CreditsManager : MonoBehaviour
    {
        private void Update()
        {
            if (InputUtil.StartJumpOrTouch()) LoadMainMenu();
        }

        private void LoadMainMenu()
        {
            SceneListManager.Instance.LoadMainMenu();
        }
    }
}
