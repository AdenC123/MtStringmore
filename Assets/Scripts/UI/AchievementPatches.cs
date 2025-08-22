using UnityEngine.UI;
using UnityEngine;

public class AchievementPatches : MonoBehaviour
{
    [SerializeField] private Image timeImage,candyImage,deathImage;
    [SerializeField] private Sprite defaultTime, defaultDeath, timePatch, candyPatch, deathPatch;
    private Sprite defaultCandy; //not serialized because passed in as a param.
    [SerializeField, Tooltip("Time to bead to gain achievementPatch in seconds")] 
    private float level1Threshold, level2Threshold, level3Threshold, level4Threshold;

    /// <summary>
    /// Displays achievement patches if applicable, called by Level Select line 61
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
        //sets the candy image (because it changes depending on level)
        defaultCandy = levelCandy;
        
        //check default value for candy
        if (!candiesInLevel.Equals(-1))
            candyImage.sprite = candiesInLevel - candiesCollected == 0 ? candyPatch : defaultCandy;
        
        deathImage.sprite = numDeaths == 0 ? deathPatch : defaultDeath;
        
        //check default value for time
        if (float.IsNaN(timeTaken))
            timeImage.sprite = defaultTime;
        else
            timeImage.sprite = WithinTimeThreshold(level, timeTaken)? timePatch : defaultTime;
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
    
}
