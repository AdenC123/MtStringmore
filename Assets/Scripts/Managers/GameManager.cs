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
        /// Action invoked on game data changed.
        /// </summary>
        public event Action GameDataChanged;

        /// <summary>
        /// Canvas to fade in/out when transitioning between scenes
        /// </summary>
        [SerializeField] private FadeEffects sceneTransitionCanvas;

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
        // We are resetting all stats (collectables etc.) each time we load scene
        // We need make sure we are not clearing stats when we are loading the results after a cutscene
        // Therefore we need a list of all cutscenes that show results after the cutscene
        // Results Manager also uses this to avoid issues
        // <summary>
        [SerializeField] private List<string> cutsceneList;

        private void Awake()
        {
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
            _dontClearDataOnSceneChanged = cutsceneList.Contains(scene.name);


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
                GameDataChanged?.Invoke();
            }
            
            Collectable[] collectables = FindObjectsOfType<Collectable>();
            
            if (!_dontClearDataOnSceneChanged)
            {
                _collectableLookup.Clear();
                MaxCollectablesCount = collectables.Length;
                Debug.Log("GameManager loaded " + MaxCollectablesCount + " collectables");
            }
            else
            {
                Debug.Log("Skipping collectable count in cutscene. Using previous value: " + MaxCollectablesCount);
            }
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

        //<summary>
        //grabs the total number of collectables
        //called by the results manager
        //<summary>
        public int findTotalCollectables()
        {
            return FindObjectsOfType<Collectable>().Length;
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
            GameDataChanged?.Invoke();
        }

        /// <summary>
        /// Sets game information from save data.
        /// </summary>
        /// <param name="saveData">Save data from file</param>
        public void UpdateFromSaveData(SaveData saveData)
        {
            Vector2[] checkpointsReached = saveData.checkpointsReached;
            bool shouldFaceLeft = saveData.checkpointFacesLeft;
            if (checkpointsReached.Length > 0) CheckPointPos = checkpointsReached[^1];
            RespawnFacingLeft = shouldFaceLeft;
            CheckpointsReached.Clear();
            _prevCheckpoints.Clear();
            CheckpointsReached.AddRange(checkpointsReached);
            foreach (Vector2 checkpointReached in checkpointsReached)
                _prevCheckpoints.Add(checkpointReached);
            _collectedCollectables.Clear();
            foreach (Vector2 candyCollected in saveData.candiesCollected)
                _collectedCollectables.Add(candyCollected);
    
            LevelsAccessed.AddRange(saveData.levelsAccessed);

            GameDataChanged?.Invoke();
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
            GameDataChanged?.Invoke();
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
