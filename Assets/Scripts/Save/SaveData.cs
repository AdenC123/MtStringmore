using System;
using System.Collections.Generic;

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

        public LevelData level1Data;
        public LevelData level2Data;
        public LevelData level3Data;
        public LevelData level4Data;
    }

    [Serializable]
    public struct LevelData
    {
        public int mostCandies;
        public int leastDeaths;
        public string bestTime;
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
