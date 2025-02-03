using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

/// <summary>
///     Singleton class for global game settings. Persists between scenes and reloads.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    /// <summary>
    ///     Last checkpoint position. The player should respawn here if they die.
    /// </summary>
    public Vector2 CheckPointPos { get; set; }
    
    /// <summary>
    /// If true, the player faces left when they respawn
    /// </summary>
    public bool RespawnFacingLeft { get; set; }

    /// <summary>
    /// Object that contains all objects that need to be reset when the player respawns
    /// </summary>
    private List<Resettable> _resetters = new();

    private void Awake()
    {
        if (Instance == null)
        {
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;
        foreach (var resetObject in GameObject.FindGameObjectsWithTag("Resetter"))
            _resetters.Add(resetObject.GetComponent<Resettable>());
    }

    /// <summary>
    /// Resets core components like player, Knitby, camera, etc to the state at
    /// the last checkpoint
    /// </summary>
    public void Respawn()
    {
        foreach (var resetObject in _resetters)
            resetObject.Reset();
    }

    [YarnCommand("load_scene")]
    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}