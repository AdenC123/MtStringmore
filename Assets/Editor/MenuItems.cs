﻿using Interactables;
using UnityEditor;
using UnityEngine;

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
    }
}
