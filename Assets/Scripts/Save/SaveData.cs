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
        public bool checkpointFacesLeft;
        public List<string> levelsAccessed;
    }

    /// <summary>
    /// Data saved in the save file.
    /// </summary>
    [Serializable]
    public struct SaveFileData
    {
        public long[] fastestTimes;
        public SaveData saveData;
    }
}
