using Managers;
using Save;
using UI;
using UnityEngine.UI;
using UnityEngine;

namespace UI
{
    public class AchievementPatches : MonoBehaviour
    {
        [SerializeField] private Image timeImage, candyImage, deathImage, playPanel;

        [SerializeField]
        private Sprite defaultTime, defaultDeath, timePatch, candyPatch, deathPatch, goldPanel, defaultPanel;

        [SerializeField, Tooltip("Time to bead to gain achievementPatch in seconds")]
        private float level1Threshold, level2Threshold, level3Threshold, level4Threshold;

        /// <summary>
        /// A check for whether the panel should be gold for each level
        /// </summary>
        public bool isGoldL1 { get; private set; } = false;

        public bool isGoldL2 { get; private set; } = false;
        public bool isGoldL3 { get; private set; } = false;
        public bool isGoldL4 { get; private set; } = false;

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
            bool timeCondition = !float.IsNaN(timeTaken) && WithinTimeThreshold(level, timeTaken);

            //check default value for candy
            if (candiesInLevel != -1)
                candyImage.sprite = candyCondition ? candyPatch : levelCandy;

            deathImage.sprite = deathCondition ? deathPatch : defaultDeath;

            //check default value for time
            if (!float.IsNaN(timeTaken))
                timeImage.sprite = timeCondition ? timePatch : defaultTime;
            else
                timeImage.sprite = defaultTime;

            //apply gold to playPanel if applicable
            if (timeCondition && candyCondition && deathCondition)
                playPanel.sprite = goldPanel;
            else
                playPanel.sprite = defaultPanel;
        }

        private bool WithinTimeThreshold(int level, float timeTaken)
        {
            return level switch
            {
                1 => timeTaken < level1Threshold,
                2 => timeTaken < level2Threshold,
                3 => timeTaken < level3Threshold,
                4 => timeTaken < level4Threshold,
                _ => false
            };
        }

        /// <summary>
        /// Determines whether we meet the 3 conditions of setting the button gold:
        /// 1. Under time threshold
        /// 2. All Candies Collected
        /// 3. Zero deaths
        /// called from LevelSelect line 46
        /// </summary>
        /// <param name="btn">Candidate button for turning gold</param>
        /// <param name="level">Level number</param>
        public void CheckSetGoldButton(LevelSelectButton btn, int level)
        {
            GameManager gm = GameManager.Instance;
            Debug.Log(level);

            //make sure level is within correct range
            if (1 > level || level > 4) return;
            LevelData thisLevel;
            thisLevel = gm.AllLevelData[level - 1];

            //check defaults, then check if we meet all requirement
            if (!thisLevel.totalCandiesInLevel.Equals(-1) &&
                !thisLevel.leastDeaths.Equals(-1) &&
                !float.IsNaN(thisLevel.bestTime))
            {
                if (WithinTimeThreshold(level, thisLevel.bestTime) &&
                    thisLevel.mostCandiesCollected - thisLevel.totalCandiesInLevel == 0 &&
                    thisLevel.leastDeaths == 0)
                {
                    btn.SetGold();
                    SetVariableGold(level);
                }
            }
        }

        private void SetVariableGold(int level)
        {
            switch (level)
            {
                case 1:
                    isGoldL1 = true;
                    break;
                case 2:
                    isGoldL2 = true;
                    break;
                case 3:
                    isGoldL3 = true;
                    break;
                case 4:
                    isGoldL4 = true;
                    break;
            }
        }
    }
}
