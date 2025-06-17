using Managers;
using UnityEngine;

namespace Save
{
    public class LastCheckpoint : MonoBehaviour
    {
        [SerializeField] private string nextLevel;
        public void UpdateLevelAccess()
        {
            GameManager.Instance.LevelsAccessed.Add(nextLevel);
            FindObjectOfType<SaveDataManager>()?.SaveFile();
            Debug.Log("Unlocked: " + nextLevel);
        }
    }
}
