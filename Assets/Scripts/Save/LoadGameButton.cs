using UnityEngine;

namespace Save
{
    /// <summary>
    /// Load game button logic.
    /// </summary>
    public class LoadGameButton : MonoBehaviour
    {
        private SaveDataManager _saveDataManager;

        private void Awake()
        {
            _saveDataManager = FindObjectOfType<SaveDataManager>();
            if (!SaveDataManager.HasExistingSave())
            {
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Called by the button: loads the existing save.
        /// </summary>
        public void LoadFromSave()
        {
            _saveDataManager.LoadExistingSave();
        }
    }
}
