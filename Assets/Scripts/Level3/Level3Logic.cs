using Managers;
using UnityEngine;
using UnityEngine.Events;

namespace Level3
{
    public class Level3Logic : MonoBehaviour
    {
        [SerializeField] private UnityEvent onSecondHalfReached;

        private void Start()
        {
            // there's no guarantee we grab the right instance in Awake so we use Start
            GameManager.Instance.AreInteractablesEnabled = false;
        }


        public void ReachSecondHalf()
        {
            GameManager.Instance.AreInteractablesEnabled = true;
            onSecondHalfReached.Invoke();
        }
    }
}
