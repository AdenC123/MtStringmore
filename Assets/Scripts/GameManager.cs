using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

/// <summary>
///     Singleton class for global game settings. Persists between scenes and reloads.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    ///     Used between scene transitions. Set to false for respawns, true for transition between levels
    /// </summary>
    private bool _newLevel;

    public static GameManager Instance { get; private set; }

    /// <summary>
    ///     Last checkpoint position. The player should respawn here if they die.
    /// </summary>
    public Vector2 CheckPointPos { get; set; }

    /// <summary>
    /// Object that contains all objects that need to be reset when the player respawns
    /// </summary>
    private Resettable _resetter;

    /// <summary>
    /// Name of the object that resets the <see cref="_resetter"/>
    /// </summary>
    [SerializeField] private string resetterName = "Resettable";

    private void Awake()
    {
        if (Instance == null)
        {
            _newLevel = true;
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Debug Reset")) Respawn();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    ///     On scene load, put player in the right spawn/respawn point
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _resetter = GameObject.Find(resetterName).GetComponent<Resettable>();
        
        Time.timeScale = 1f;
    }

    public void Respawn()
    {
        // _newLevel = false;
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        _resetter.Reset();
    }

    [YarnCommand("load_scene")]
    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}