using System.Linq;
using Managers;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    /// <summary>
    /// Custom logic to enable an object if all levels are gold.
    /// </summary>
    public class ThankYou : MonoBehaviour
    {
        [SerializeField] private UnityEvent eventOnAllGold;

        private void Start()
        {
            bool isAllGold = GameManager.Instance.AllLevelData.Select((data, index) => (data, index))
                .All(tuple => tuple.data.IsLevelGold(tuple.index + 1));
            if (isAllGold)
            {
                eventOnAllGold.Invoke();
            }
        }
    }
}
