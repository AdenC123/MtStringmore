using System;
using Managers;

namespace Save
{
    /// <summary>
    /// Save data.
    /// </summary>
    [Serializable]
    public struct SaveData
    {
        public long dateTimeBinary;
        public string[] levelsAccessed;

        public LevelData level1Data;
        public LevelData level2Data;
        public LevelData level3Data;
        public LevelData level4Data;
    }

    [Serializable]
    public class LevelData
    {
        public int mostCandiesCollected = -1;
        public int totalCandiesInLevel = -1;
        public int leastDeaths = -1;
        public float bestTime = float.NaN;

        /// <summary>
        /// Determines if this level data, provided a level number, would be gold.
        /// There are three conditions for a level to be gold:
        /// 1. Under time threshold (stored in SceneListManager)
        /// 2. All Candies Collected
        /// 3. Zero deaths
        /// </summary>
        /// <param name="levelNumber">Level number (1-NumLevels)</param>
        /// <returns>True if the level is gold</returns>
        public bool IsLevelGold(int levelNumber)
        {
            return leastDeaths == 0 &&
                   totalCandiesInLevel >= 0 && // if -1, level hasn't been accessed
                   mostCandiesCollected == totalCandiesInLevel &&
                   SceneListManager.Instance.IsTimeWithinThreshold(levelNumber, bestTime); // they check NaN
        }
    }

    /// <summary>
    /// Data saved in the save file.
    /// </summary>
    [Serializable]
    public struct SaveFileData
    {
        public SaveData saveData;
    }
}
