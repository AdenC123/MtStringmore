using System;
using System.Collections;
using System.Collections.Generic;
using Interactables;
using Player;
using Save;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;
using Action = System.Action;

namespace Managers
{
    /// <summary>
    /// Singleton class for global game settings. Persists between scenes and reloads.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;

        /// <summary>
        /// Finds the singleton in the scene if present.
        /// </summary>
        /// <remarks>
        /// Bypasses the lifetime check since it's DontDestroyOnLoad.
        /// </remarks>
        public static GameManager Instance => _instance ??= FindObjectOfType<GameManager>();

        /// <summary>
        /// Last checkpoint position. The player should respawn here if they die.
        /// </summary>
        public Vector2 CheckPointPos { get; private set; }

        /// <summary>
        /// If true, the player faces left when they respawn
        /// </summary>
        public bool RespawnFacingLeft { get; private set; }
    
        /// <summary>
        /// Number of checkpoints reached.
        /// </summary>
        public List<Vector2> CheckpointsReached { get; } = new();
        
        /// <summary>
        /// Number of checkpoints reached.
        /// </summary>
        public List<string> LevelsAccessed { get; } = new();
    
        /// <summary>
        /// The number of collectables collected.
        /// Should be reset to 0 after being displayed (e.g. after a end-of-level cutscene).
        /// </summary>
        public int NumCollectablesCollected => _collectedCollectables.Count;
        
        /// <summary>
        /// Max number of known collectables.
        /// </summary>
        public int MaxCollectablesCount { get; private set; }

        public IReadOnlyCollection<Vector2> CollectablePositionsCollected => _collectedCollectables;

        /// <summary>
        /// Action that gets invoked when level reloads, e.g. respawns
        /// </summary>
        public event Action Reset;

        /// <summary>
        /// Action invoked when a level is completed.
        /// </summary>
        public event Action saveGame;

        /// <summary>
        /// Canvas to fade in/out when transitioning between scenes
        /// </summary>
        [SerializeField] private FadeEffects sceneTransitionCanvas;

        [SerializeField] private List<string> levelNameList;

        private int thisLevelDeaths;
        private string thisLevelTime;
        

        private readonly Dictionary<Vector2, Collectable> _collectableLookup = new();
        private readonly HashSet<Vector2> _prevCheckpoints = new();
        private readonly HashSet<Vector2> _collectedCollectables = new();
    
        /// <summary>
        /// So it turns out that onSceneChanged happens after modifying game data on save.
        ///
        /// So we need to check that we're doing that LOL.
        /// </summary>
        private bool _dontClearDataOnSceneChanged;

        // <summary>
        // saving the level data to here so it's easier to load.
        // level data stores: int leastDeaths, int mostCandiesCollected, int totalCandiesInLevel, string bestTime
        // </summary>
        public List<LevelData> allLevelData = new List<LevelData>();

        private void Awake()
        {
            // make sure list has 4 entries
            for (int i = allLevelData.Count; i < 4; i++) {
                allLevelData.Add(new LevelData());
            }
            
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
        
            _instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            if (SystemInfo.deviceType == DeviceType.Handheld)
            {
                // TODO make maxFrameRate a setting
                Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
            }
            Debug.Log("Application version: " + Application.version);
        }

        private void Update()
        {
            if (Input.GetButtonDown("Debug Reset") && !ResultsManager.isResultsPageOpen) Respawn();
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            sceneTransitionCanvas.InvokeFadeOut();
            Time.timeScale = 1f;
            if (!_dontClearDataOnSceneChanged)
            {
                PlayerController player = FindObjectOfType<PlayerController>();
                if (player)
                {
                    CheckPointPos = player.transform.position;
                    Debug.Log("Hopefully set checkpoint position to be player's position: " + CheckPointPos);
                }
                CheckpointsReached.Clear();
                _prevCheckpoints.Clear();
                _collectedCollectables.Clear();
            }
            _collectableLookup.Clear();
            Collectable[] collectables = FindObjectsOfType<Collectable>();
            MaxCollectablesCount = collectables.Length;
            foreach (Collectable collectable in collectables)
            {
                Vector2 position = collectable.transform.position;
                if (_collectedCollectables.Contains(position))
                {
                    Destroy(collectable.gameObject);
                }
                else
                {
                    _collectableLookup.Add(position, collectable);
                }
            }
            _dontClearDataOnSceneChanged = false;
        }
        
        // <summary>
        // signals that the level is completed and the level data should be saved
        // called by resultsManager once last checkpoint is reached
        // </summary>
        public void LevelCompleted()
        {
            SaveLevelDataToGameManager();
            saveGame?.Invoke();
        }

        // <summary>
        // saves all of the current game stats (thisLevelDeaths, thisLevelCandies, thisLevelTime)
        // to the game manager level data variables
        // </summary>
        private void SaveLevelDataToGameManager()
        {
            string thisSceneName = SceneManager.GetActiveScene().name;
            int idx = -1; //baseValue
            if (!levelNameList.Contains(thisSceneName))
            {
                Debug.Log("GameManager could not determine what level we are currently in");
                return;
            }
            idx = levelNameList.IndexOf(thisSceneName);
            SaveToCorrectLevelVariable(idx); //level number - 1 is the index b/c 0-based indexing
        }

        private bool BeatsCurrentTime(string currBestTimeSpan, string newTimeSpan)
        {
            if (currBestTimeSpan == "--:--:--")
                return true;
            if (newTimeSpan == "--:--:--")
                return false;
            TimeSpan t1 = TimeSpan.Parse(currBestTimeSpan);
            TimeSpan t2 = TimeSpan.Parse(newTimeSpan);
            return t2 < t1;
        }

        private void SaveToCorrectLevelVariable(int index)
        {
            if (index >= 0 && index < allLevelData.Count)
            {
                LevelData updatedLevelData = allLevelData[index];
                if (updatedLevelData.mostCandiesCollected < _collectedCollectables.Count)
                    updatedLevelData.mostCandiesCollected = _collectedCollectables.Count;
                updatedLevelData.totalCandiesInLevel = _collectableLookup.Count;
                if (updatedLevelData.leastDeaths > thisLevelDeaths)
                    updatedLevelData.leastDeaths = thisLevelDeaths;
                if (BeatsCurrentTime(updatedLevelData.bestTime, thisLevelTime))
                    updatedLevelData.bestTime = thisLevelTime;

                allLevelData[index] = updatedLevelData;
            }
            else
            {
                Debug.Log("Could not save data to GameManager");
            }
        }

        /// <summary>
        /// Sets the checkpoint location and left facing spawn point.
        /// 
        /// Checks if we've already visited this checkpoint before setting spawn (to avoid backtracking).
        /// </summary>
        /// <param name="newCheckpointLocation">New checkpoint location</param>
        /// <param name="shouldFaceLeft">Whether respawn should face left</param>
        public void UpdateCheckpointData(Vector2 newCheckpointLocation, bool shouldFaceLeft = false)
        {
            if (!_prevCheckpoints.Add(newCheckpointLocation))
            {
                Debug.Log("Reached previous checkpoint.");
                return;
            }

            CheckPointPos = newCheckpointLocation;
            RespawnFacingLeft = shouldFaceLeft;
            CheckpointsReached.Add(newCheckpointLocation);
        }

        /// <summary>
        /// Sets game information from save data.
        /// </summary>
        /// <param name="saveData">Save data from file</param>
        public void UpdateFromSaveData(SaveData saveData)
        {
            bool shouldFaceLeft = saveData.checkpointFacesLeft;
            RespawnFacingLeft = shouldFaceLeft;
            CheckpointsReached.Clear();
            _prevCheckpoints.Clear();
            _collectedCollectables.Clear();
            LevelsAccessed.AddRange(saveData.levelsAccessed);

            saveGame?.Invoke();
            _dontClearDataOnSceneChanged = true;
        }

        /// <summary>
        /// Increments the number of candy collected.
        /// </summary>
        public void CollectCollectable(Collectable collectable)
        {
            _collectedCollectables.Add(collectable.transform.position);
        }

        /// <summary>
        /// Resets the number of candy collected.
        /// </summary>
        public void ResetCandyCollected()
        {
            _collectedCollectables.Clear();
        }

        /// <summary>
        /// Resets core components like player, Knitby, camera, etc to the state at
        /// the last checkpoint
        /// </summary>
        public void Respawn()
        {
            sceneTransitionCanvas.FadeIn += OnFadeIn;
            sceneTransitionCanvas.InvokeFadeInAndOut();
        }

        private void OnFadeIn()
        {
            Reset?.Invoke();
            sceneTransitionCanvas.FadeIn -= OnFadeIn;
        }

        [YarnCommand("load_scene_nonblock")]
        public void InvokeLoadScene(string sceneName, float duration = 0)
        {
            StartCoroutine(LoadScene(sceneName, duration));
        }
    
        [YarnCommand("load_scene")]
        public static IEnumerator LoadScene(string sceneName, float duration = 0)
        {
            yield return new WaitForSecondsRealtime(duration);
            SceneManager.LoadScene(sceneName);
        }
    }
}
