using System;
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
        public long timeTaken;
        public string sceneName;
        public bool checkpointFacesLeft;
        public Vector2[] checkpointsReached;
    }

    /// <summary>
    /// Data saved in the save file.
    /// </summary>
    [Serializable]
    public struct SaveFileData
    {
        public string farthestSceneReached;
        public long[] fastestTimes;
        public SaveData saveData;
    }
}
