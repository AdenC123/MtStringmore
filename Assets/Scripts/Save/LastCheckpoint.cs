using Managers;
using UnityEngine;

namespace Save
{
    public class LastCheckpoint : MonoBehaviour
    {
        [SerializeField] private string nextLevel;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                GameManager.Instance.LevelsAccessed.Add(nextLevel);
        }
    }
}
