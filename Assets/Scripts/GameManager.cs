using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    public Vector2 CheckPointPos { get; set; }
    private bool _newLevel;
    private bool _gamePaused;

    // Array to hold references to all AudioSource components
    private AudioSource[] _audioSources;

    private void Awake() {
        if (Instance == null) {
            _newLevel = true;
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;

            // Get all AudioSource components in the AudioManager GameObject
            _audioSources = FindObjectsOfType<AudioSource>();
        } else {
            Destroy(gameObject);
        }
    }

    private void Update() {
        if (Input.GetButtonDown("Debug Reset")) {
            Respawn();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            _gamePaused = !_gamePaused;
            if (_gamePaused) {
                PauseGame();
            } else {
                ResumeGame();
            }
        }
    }

    void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (!_newLevel) {
            Vector3 spawnPos = new Vector3(CheckPointPos.x, CheckPointPos.y, player.transform.position.z);
            player.transform.position = spawnPos;
        } else {
            CheckPointPos = player.transform.position;
        }

        FollowCamera cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<FollowCamera>();
        Vector2 playerTarget = cam.GetPlayerTarget();
        cam.transform.position = new Vector3(playerTarget.x, playerTarget.y, cam.transform.position.z);
    }

    public void PauseGame() {
        Time.timeScale = 0;

        // Mute all audio sources
        foreach (var audioSource in _audioSources) {
            audioSource.Pause();
            audioSource.volume = 0;
        }
    }

    public void ResumeGame() {
        Time.timeScale = 1;

        // Restore audio volume
        foreach (var audioSource in _audioSources) {
            audioSource.volume = 1;
            audioSource.UnPause();
        }
    }

    public void Respawn() {
        _newLevel = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
