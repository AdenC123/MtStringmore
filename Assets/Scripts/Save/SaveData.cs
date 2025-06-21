using System;
using System.Collections.Generic;
using UnityEngine;

namespace Save
{
    /// <summary>
    /// Save data.
    /// </summary>
    [Serializable]
    public struct SaveData
    {
        public long dateTimeBinary;
        public float timeTaken;
        public Vector2[] candiesCollected;
        public string sceneName;
        public bool checkpointFacesLeft;
        public Vector2[] checkpointsReached;
        public List<string> levelsAccessed;
    }

    /// <summary>
    /// Data saved in the save file.
    /// </summary>
    [Serializable]
    public struct SaveFileData
    {
        public string farthestSceneReached;
        public int farthestSceneIndexReached;
        public float[] fastestTimes;
        public int[][] levelCandies;
        public SaveData saveData;
    }
}
