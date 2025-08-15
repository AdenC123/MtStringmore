using Managers;
using UnityEngine;

namespace Save
{
    public class LastCheckpoint : MonoBehaviour
    {
        [SerializeField] private string nextLevel;
        private SaveDataManager saveManager;

        private void Start()
        {
            saveManager = FindObjectOfType<SaveDataManager>();
        }
        
        public void UpdateLevelAccess()
        {
            if (!GameManager.Instance.AddLevelAccessed(nextLevel))
            {
                saveManager?.SaveFile();
                Debug.Log("Unlocked: " + nextLevel);
            }
        }
    }
}
