using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using Yarn.Unity;

public class CutsceneCamera : MonoBehaviour
{
    [CanBeNull] private static GameObject _targetObject;
    private static Vector2 _target;
    private static Camera _camera;

    [SerializeField] private float xSmoothTime;
    [SerializeField] private float ySmoothTime;
    private float _xVelocity;
    private float _yVelocity;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (_targetObject) _target = _targetObject.transform.position;

        // apply smoothing to the camera
        Vector3 camPosition = transform.position;
        float smoothedX = Mathf.SmoothDamp(camPosition.x, _target.x, ref _xVelocity, xSmoothTime);
        float smoothedY = Mathf.SmoothDamp(camPosition.y, _target.y, ref _yVelocity, ySmoothTime);
        transform.position = new Vector3(smoothedX, smoothedY, camPosition.z);
    }


    [YarnCommand("follow_object")]
    public static void FollowObject(GameObject target)
    {
        _targetObject = target;
        _target = target.transform.position;
    }

    [YarnCommand("fix_coords")]
    public static void FixCoords(float x, float y)
    {
        _targetObject = null;
        _target = new Vector2(x, y);
    }

    [YarnCommand("fix_object")]
    public static void FixObject(GameObject obj)
    {
        FixCoords(obj.transform.position.x, obj.transform.position.y);
    }

    [YarnCommand("resize_camera")]
    public void ResizeCamera(float size, float speed)
    {
        StartCoroutine(ResizeCameraBlock(size, speed));
    }

    [YarnCommand("resize_camera_block")]
    public static IEnumerator ResizeCameraBlock(float size, float speed)
    {
        while (!Mathf.Approximately(_camera.orthographicSize, size))
        {
            float newSize = Mathf.MoveTowards(_camera.orthographicSize, size, Time.deltaTime * speed);
            _camera.orthographicSize = newSize;
            yield return null;
        }
    }
}
