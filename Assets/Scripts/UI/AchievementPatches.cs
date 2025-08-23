using Managers;
using Save;
using UI;
using UnityEngine.UI;
using UnityEngine;

public class AchievementPatches : MonoBehaviour
{
    [SerializeField] private Image timeImage,candyImage,deathImage, playPanel;
    [SerializeField] private Sprite defaultTime, defaultDeath, timePatch, candyPatch, deathPatch, goldPanel, defaultPanel;
    private Sprite defaultCandy; //not serialized because passed in as a param.
    [SerializeField, Tooltip("Time to bead to gain achievementPatch in seconds")] 
    private float level1Threshold, level2Threshold, level3Threshold, level4Threshold;
    
    /// <summary>
    /// A check for whether the panel should be gold for each level
    /// </summary>
    private bool isGoldl1, isGoldl2, isGoldl3, isGoldl4;

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
        bool candyCondition = candiesInLevel - candiesCollected == 0;
        bool deathCondition = numDeaths == 0;
        bool timeCondition = WithinTimeThreshold(level, timeTaken);
        
        //sets the candy image (because it changes depending on level)
        defaultCandy = levelCandy;
        
        //check default value for candy
        if (!candiesInLevel.Equals(-1))
            candyImage.sprite = candyCondition ? candyPatch : defaultCandy;
        
        deathImage.sprite = deathCondition? deathPatch:defaultDeath;
        
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
        LevelData thisLevel = gm.AllLevelData[level - 1];

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
                isGoldl1 = true;
                break;
            case 2:
                isGoldl2 = true;
                break;
            case 3:
                isGoldl3 = true;
                break;
            case 4:
                isGoldl4 = true;
                break;
        }
    }
}
