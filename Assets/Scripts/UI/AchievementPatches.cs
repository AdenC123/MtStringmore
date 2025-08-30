using Managers;
using UnityEngine.UI;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Custom logic to handle accomplishing certain achievements (time less than threshold, no deaths/all candies).
    /// </summary>
    public class AchievementPatches : MonoBehaviour
    {
        [SerializeField] private Image timeImage, candyImage, deathImage, playPanel;

        [SerializeField]
        private Sprite defaultTime, defaultDeath, timePatch, candyPatch, deathPatch, goldPanel, defaultPanel;

        /// <summary>
        /// Displays achievement patches if applicable, called by Level Select line 97
        /// </summary>
        /// <param name="level">Level number</param>
        /// <param name="timeTaken">Time taken to complete level in seconds</param>
        /// <param name="candiesCollected">Highest number of candies collected in level</param>
        /// <param name="candiesInLevel">Total number of candies in level</param>
        /// <param name="numDeaths">Lowest number of deaths in level</param>
        /// <param name="levelCandy">The candy sprite for level</param>
        public void DisplayAchievementPatches(int level, float timeTaken, int candiesCollected, int candiesInLevel, int
            numDeaths, Sprite levelCandy)
        {
            //conditions to meet to get patch
            bool candyCondition = candiesInLevel != -1 && candiesCollected == candiesInLevel;
            bool deathCondition = numDeaths == 0;
            bool timeCondition = SceneListManager.Instance.IsTimeWithinThreshold(level, timeTaken);

            candyImage.sprite = candyCondition ? candyPatch : levelCandy;
            deathImage.sprite = deathCondition ? deathPatch : defaultDeath;
            timeImage.sprite = timeCondition ? timePatch : defaultTime;

            //apply gold to playPanel if applicable
            playPanel.sprite = timeCondition && candyCondition && deathCondition ? goldPanel : defaultPanel;
        }
    }
}
