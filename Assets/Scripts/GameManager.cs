using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    public Vector2 CheckPointPos { get; set; }
    private bool _newLevel;
    private void Awake() {
        if (Instance == null) {
            _newLevel = true;
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;

        } else {
            Destroy(gameObject);
        }
    }

    private void Update() {
        if (Input.GetButtonDown("Debug Reset")) {
            Respawn();
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

    public void Respawn() {
        _newLevel = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
