using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    /// <summary>
    /// Main source of truth for scene information.
    /// </summary>
    public class SceneListManager : MonoBehaviour
    {
        private static SceneListManager _instance;

        /// <summary>
        /// "Singleton" instance of this scene list manager
        /// </summary>
        public static SceneListManager Instance
        {
            get
            {
                if (!_instance) _instance = FindAnyObjectByType<SceneListManager>();
                return _instance;
            }
        }

        [SerializeField] private string mainMenuSceneName;
        [SerializeField] private string[] levelNameList;
        [SerializeField] private string[] cutsceneNameList;

        private readonly Dictionary<string, int> _levelLookup = new();
        private readonly HashSet<string> _cutscenes = new();

        /// <summary>
        /// Whether the current scene is the main menu.
        /// </summary>
        public bool InMainMenu => mainMenuSceneName == SceneManager.GetActiveScene().name;

        /// <summary>
        /// Whether the current scene is a cutscene.
        /// </summary>
        public bool InCutscene => IsSceneCutscene(SceneManager.GetActiveScene().name);

        /// <summary>
        /// Current level number (1-indexed).
        /// </summary>
        public int LevelNumber
        {
            get
            {
                InitializeLookupsIfNotPresent();
                string currentSceneName = SceneManager.GetActiveScene().name;
                return _levelLookup.GetValueOrDefault(currentSceneName, -1);
            }
        }

        /// <summary>
        /// Whether a given scene is the main menu scene.
        /// </summary>
        /// <param name="sceneName">Scene name</param>
        /// <returns>True if it's the main menu</returns>
        public bool IsMainMenu(string sceneName)
        {
            return sceneName == mainMenuSceneName;
        }

        /// <summary>
        /// Loads the main menu.
        /// </summary>
        public void LoadMainMenu()
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }

        /// <summary>
        /// Determines whether a given scene is a cutscene.
        /// </summary>
        /// <param name="sceneName">Given scene name</param>
        /// <returns>Whether it's a cutscene</returns>
        public bool IsSceneCutscene(string sceneName)
        {
            InitializeLookupsIfNotPresent();
            return _cutscenes.Contains(sceneName);
        }

        /// <summary>
        /// Initializes level and cutscene lookup if not present.
        /// </summary>
        private void InitializeLookupsIfNotPresent()
        {
            if (_levelLookup.Count != 0) return;
            _levelLookup.Clear();
            _cutscenes.Clear();
            for (int i = 0; i < levelNameList.Length; i++)
            {
                _levelLookup[levelNameList[i]] = i+1;
            }

            for (int i = 0; i < cutsceneNameList.Length; i++)
            {
                // level 2 has 2 cutscenes
                _levelLookup[cutsceneNameList[i]] = i+(i < 2 ? 1 : 0);
                _cutscenes.Add(cutsceneNameList[i]);
            }
        }
    }
}
