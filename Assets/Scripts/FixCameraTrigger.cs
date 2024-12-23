using UnityEngine;

/// <summary>
/// Fixes the camera at a set position when the player enters the bounding box.
/// Camera position is the centre of the box collider, plus an offset.
/// Uses "MainCamera" and "Player" tags.
/// </summary>
public class FixCameraTrigger : MonoBehaviour
{
    [SerializeField] private Vector2 targetOffset;
    [SerializeField] public bool fixX = true;
    [SerializeField] public bool fixY = true;

    public Vector2 Target => _target;

    private Vector2 _target;
    private FollowCamera _cam;

    private void Awake()
    {
        _cam = GameObject.FindWithTag("MainCamera").GetComponent<FollowCamera>();
        Vector2 boxOffset = GetComponent<BoxCollider2D>().offset;
        _target = (Vector2)transform.position + boxOffset + targetOffset;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _cam.EnterFixCameraTrigger(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _cam.ExitFixCameraTrigger(this);
        }
    }
}