using Interactables;
using Interactables.Balloon;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    /// <summary>
    /// Custom editor for the AttachableMovingObject to make it easier to position end-points.
    /// </summary>
    [CustomEditor(typeof(Balloon)), CanEditMultipleObjects]
    public class BalloonEditor : UnityEditor.Editor
    {
        private SerializedProperty firstPositionProperty;
        private SerializedProperty secondPositionProperty;
        
        private void OnEnable()
        {
            firstPositionProperty = serializedObject.FindProperty(nameof(Balloon.firstPosition));
            secondPositionProperty = serializedObject.FindProperty(nameof(Balloon.secondPosition));
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (targets.Length > 1) return;
            Balloon balloon = target as Balloon;
            if (balloon == null) return;
            serializedObject.Update();

            if (GUILayout.Button("Set First Position As Current"))
            {
                firstPositionProperty.vector2Value= balloon.transform.position;
                serializedObject.ApplyModifiedProperties();
            }

            if (GUILayout.Button("Set Second Position As Current"))
            {
                secondPositionProperty.vector2Value= balloon.transform.position;
                serializedObject.ApplyModifiedProperties();
            }

            if (GUILayout.Button("Move to first position"))
            {
                balloon.transform.position = balloon.firstPosition;
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void OnSceneGUI()
        {
            Balloon balloon = target as Balloon;
            Debug.Assert(balloon != null);

            EditorGUI.BeginChangeCheck();
            Vector2 firstPos = Handles.PositionHandle(balloon.firstPosition, Quaternion.identity);
            Vector2 secondPos = Handles.PositionHandle(balloon.secondPosition, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(balloon, "Change Hider position");
                balloon.firstPosition = firstPos;
                balloon.secondPosition = secondPos;
            }
        }
    }
}
