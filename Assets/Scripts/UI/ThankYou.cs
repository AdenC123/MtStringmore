using System.Linq;
using Managers;
using TMPro;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Custom logic to enable an object if all levels are gold.
    /// </summary>
    public class ThankYou : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        private void Start()
        {
            text.enabled = GameManager.Instance.AllLevelData.Select((data, index) => (data, index))
                .All(tuple => tuple.data.IsLevelGold(tuple.index));
        }
    }
}
