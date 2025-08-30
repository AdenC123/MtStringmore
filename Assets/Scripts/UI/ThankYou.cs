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
        [SerializeField] private AchievementPatches ap;

        private void OnEnable()
        {
            text.enabled = ap.IsAllGold;
        }
    }
}
