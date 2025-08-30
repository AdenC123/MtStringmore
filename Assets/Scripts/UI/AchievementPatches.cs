using System.Linq;
using Managers;
using Save;
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

        private readonly bool[] levelsGold = new bool[GameManager.NumLevels];

        /// <summary>
        /// Returns true if all the levels are gold.
        /// </summary>
        public bool IsAllGold => levelsGold.All(a => a);

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

        /// <summary>
        /// Determines whether we meet the 3 conditions of setting the button gold:
        /// 1. Under time threshold
        /// 2. All Candies Collected
        /// 3. Zero deaths
        /// called from LevelSelect on Start
        /// </summary>
        /// <param name="btn">Candidate button for turning gold</param>
        /// <param name="level">Level number</param>
        public void CheckSetGoldButton(LevelSelectButton btn, int level)
        {
            //make sure level is within correct range
            if (1 > level || level > GameManager.NumLevels) return;
            LevelData thisLevel = GameManager.Instance.AllLevelData[level - 1];

            //check defaults, then check if we meet all requirement
            if (thisLevel.totalCandiesInLevel == -1 || thisLevel.leastDeaths == -1 || float.IsNaN(thisLevel.bestTime))
            {
                return;
            }
            if (SceneListManager.Instance.IsTimeWithinThreshold(level, thisLevel.bestTime) &&
                thisLevel.mostCandiesCollected - thisLevel.totalCandiesInLevel == 0 &&
                thisLevel.leastDeaths == 0)
            {
                btn.SetGold();
                levelsGold[level - 1] = true;
            }
        }
    }
}
