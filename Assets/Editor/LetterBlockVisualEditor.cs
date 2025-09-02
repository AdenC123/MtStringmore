using Interactables.LetterBlock;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    /// <summary>
    /// Custom editor for letter blocks to easily randomize them.
    /// </summary>
    [CustomEditor(typeof(LetterBlockVisual))]
    public class LetterBlockVisualEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            LetterBlockVisual block = target as LetterBlockVisual;
            if (GUILayout.Button("Randomize") && block is not null)
            {
                block.RandomizeSprite();
            }
        }
    }
}
