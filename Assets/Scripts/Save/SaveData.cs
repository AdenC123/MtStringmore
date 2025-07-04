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
        
        //Might be scuffed but for the following:
        //index 0 = level 1
        //index 1 = level 2
        //index 2 = level 3
        //index 4 = level 4

        public List<int> deathCounter;
        public List<int> candyCounter;
        public List<int> fastestTimes; //in seconds
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
