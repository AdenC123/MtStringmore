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
        public string sceneName;
        public Vector2 checkpointPosition;
        public bool checkpointFacesLeft;
        public Vector2[] checkpointsReached;
    }
}
