using Interactables;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    /// <summary>
    /// Manager for editor menu items, etc.
    /// </summary>
    public class MenuItems : MonoBehaviour
    {
        /// <summary>
        /// Randomize the rotation and sprite of all collectables in the current scene.
        /// </summary>
        [MenuItem("Assets/Randomize Collectables")]
        private static void RandomizeAllCollectablesInScene()
        {
            foreach (Collectable collectable in GameObject.FindObjectsOfType<Collectable>())
            {
                collectable.RandomizeSprite();
                collectable.RandomizeRotation();
            }
        }
    }
}
