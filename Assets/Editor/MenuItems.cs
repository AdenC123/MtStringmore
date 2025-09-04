using System.Diagnostics;
using System.IO;
using Interactables;
using Interactables.LetterBlock;
using Save;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Editor
{
    /// <summary>
    /// Instantiates functions and windows to be run from the Unity editor menu.
    /// </summary>
    public class MenuItems
    {
        /// <summary>
        /// Randomize the rotation and sprite of all collectables in the current scene.
        /// </summary>
        [MenuItem("Assets/Randomize Collectables")]
        private static void RandomizeAllCollectablesInScene()
        {
            foreach (Collectable collectable in Object.FindObjectsOfType<Collectable>())
            {
                collectable.RandomizeSprite();
                collectable.RandomizeRotation();
            }
        }
        
        [MenuItem("Assets/Randomize Letter Blocks")]
        private static void RandomizeAllBlocksInScene()
        {
            foreach (LetterBlockVisual block in Object.FindObjectsOfType<LetterBlockVisual>())
            {
                block.RandomizeSprite();
            }
        }

        /// <summary>
        /// Deletes save data for testing.
        /// </summary>
        [MenuItem("Assets/Delete Save Data")]
        private static void DeleteSaveData()
        {
            SaveDataManager.DeleteSaveData();
        }
        
        /// <summary>
        /// Open save data file.
        /// </summary>
        [MenuItem("Assets/Open Save Data")]
        private static void OpenSaveData()
        {
            string saveFileLocation = SaveDataManager.SaveFileLocation;
            if (!File.Exists(saveFileLocation))
            {
                Debug.LogWarning("Save file not found.");
                return;
            }
            // hope it's not malware.
            Process.Start($"{saveFileLocation}");
        }
    }
}
