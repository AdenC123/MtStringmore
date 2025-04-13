using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Replay
{
    /// <summary>
    /// Class that constantly monitors player location and writes it to a file.
    ///
    /// If we detect we're not in the editor, it destroys itself to save on performance.
    /// </summary>
    [DisallowMultipleComponent]
    public class PlayerReplayMonitor : MonoBehaviour
    {
        /// <summary>
        /// Colors to render the different attempts with.
        /// </summary>
        private static readonly Color[] PreviewColorCycle = { Color.yellow, Color.red, Color.green, Color.blue, Color.magenta, Color.cyan };
        [SerializeField]
        private SceneReplay sceneReplayPreview;

        [SerializeField, Tooltip("Warning: likely performance intensive!")] private bool showPreview = true;
    
        private PlayerController _player;
        private string _activeSceneName;
        private readonly List<SceneReplay.Attempt> _prevAttempts = new();
        private readonly List<Vector3> _currAttempt = new();

        private void Awake()
        {
            if (!Application.isEditor)
            {
                Debug.LogWarning("PlayerReplayMonitor is only supported on Editor.");
                Destroy(this);
            }

            _player = FindObjectOfType<PlayerController>();

            SceneManager.sceneLoaded += OnSceneLoaded;
            _activeSceneName = SceneManager.GetActiveScene().name;
            _prevAttempts.Clear();
            _currAttempt.Clear();
            GameManager.Instance.Reset += OnReset;
        }

        private void FixedUpdate()
        {
            _currAttempt.Add(_player.transform.position);
        }

        private void OnDestroy()
        {
            AddCurrentAttempt();
            WriteOutSceneReplay();
        }

        private void OnDrawGizmos()
        {
            if (!showPreview) return;
            if (sceneReplayPreview)
            {
                for (int i = 0; i < sceneReplayPreview.attempts.Length; i++)
                {
                    if (sceneReplayPreview.attempts[i].locations.Length < 2) continue;
                    Gizmos.color = PreviewColorCycle[i % PreviewColorCycle.Length];
                    Gizmos.DrawLineStrip(sceneReplayPreview.attempts[i].locations, false);
                }
            }
            else
            {
                for (int i = 0; i < _prevAttempts.Count; i++)
                {
                    if (_prevAttempts[i].locations.Length < 2) continue;
                    Gizmos.color = PreviewColorCycle[i % PreviewColorCycle.Length];
                    Gizmos.DrawLineStrip(_prevAttempts[i].locations, false);
                }

                if (_currAttempt.Count >= 2)
                {
                    Gizmos.color = PreviewColorCycle[_prevAttempts.Count % PreviewColorCycle.Length];
                    // note: VERY performance intensive
                    Gizmos.DrawLineStrip(_currAttempt.ToArray(), false);
                }
            }
        }

        /// <summary>
        /// Called on reset: saves the attempt.
        /// </summary>
        private void OnReset()
        {
            AddCurrentAttempt();
        }

        /// <summary>
        /// Called on scene load: writes out the previous scene data to a file (if there were attempts/data).
        /// </summary>
        /// <param name="scene">New scene</param>
        /// <param name="loadSceneMode">Scene load mode</param>
        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (_currAttempt.Count > 0) AddCurrentAttempt();
            if (_currAttempt.Count != 0 && _prevAttempts.Count != 0) WriteOutSceneReplay();
            _activeSceneName = scene.name;
            _currAttempt.Clear();
            _prevAttempts.Clear();
            _player = FindObjectOfType<PlayerController>();
        }

        /// <summary>
        /// Adds the current attempt to the list of previous attempts.
        /// </summary>
        private void AddCurrentAttempt()
        {
            _prevAttempts.Add(new SceneReplay.Attempt { locations = _currAttempt.ToArray() });
            _currAttempt.Clear();
        }

        /// <summary>
        /// Writes out the scene replay.
        /// </summary>
        /// <remarks>
        /// If we're not in the editor, it doesn't do anything.
        /// </remarks>
        private void WriteOutSceneReplay()
        {
            SceneReplay sceneReplay = ScriptableObject.CreateInstance<SceneReplay>();
            sceneReplay.sceneName = _activeSceneName;
            sceneReplay.attempts = _prevAttempts.ToArray();
            string filePath = $"Assets/Editor/Replays/Replay-{_activeSceneName}-{DateTime.Now.ToString("s").Replace(':','-')}.asset";
#if UNITY_EDITOR
            if (!AssetDatabase.IsValidFolder("Assets/Editor/Replays"))
                AssetDatabase.CreateFolder("Assets/Editor", "Replays");
            AssetDatabase.CreateAsset(sceneReplay, filePath);
            AssetDatabase.SaveAssets();
            Debug.Log($"Saved replay to {filePath}");
#endif
        }
    }
}
