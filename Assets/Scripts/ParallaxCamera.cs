using System;
using UnityEngine;

/// <summary>
/// Creates tiling background and moves them according to their parallax factors as the camera moves
/// </summary>
public class ParallaxCamera : MonoBehaviour
{
    #region Properties
    [SerializeField] private float scrollSpeed;
    private Camera _mainCamera;
    private float _screenWidth;
    private float _prevX;
    private float _prevY;
    private GameObject[] _layers;
    
    /// <summary>
    /// Fired when camera moves.
    /// Parameter is the amount the camera moved on the x-axis since the last frame.
    /// </summary>
    public event Action<float, float> Moved;
    
    #endregion
    
    #region Unity Event Handlers

    private void Start()
    {
        Camera cam = gameObject.GetComponent<Camera>();
        float height = 2f * cam.orthographicSize;
        _screenWidth = height * cam.aspect;
        
        GameObject background = GameObject.FindGameObjectWithTag("ParallaxBackground");
        _layers = new GameObject[background.transform.childCount];
        int i = 0;
        foreach (Transform child in background.transform)
        {
            _layers[i] = child.gameObject;
            i++;
        }
    }

    private void Update()
    {
        Vector3 velocity = Vector3.zero;
        Vector3 desiredPosition = transform.position + new Vector3(scrollSpeed, 0, 0);
        Vector3 smoothPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 0.3f);
        transform.position = smoothPosition;
    }

    private void LateUpdate()
    {
        if (!Mathf.Approximately(_prevX, transform.position.x) || !Mathf.Approximately(_prevY, transform.position.y))
        {
            Moved?.Invoke(_prevX - transform.position.x, _prevY - transform.position.y);
            _prevX = transform.position.x;
            _prevY = transform.position.y;
        }
        
        foreach (GameObject layer in _layers)
        {
            RepositionLayer(layer);
        }
    }
    
    #endregion
    
    #region Helper Functions

    private void RepositionLayer(GameObject obj)
    {
        float bgWidth = obj.GetComponent<SpriteRenderer>().bounds.size.x;
        
        if (obj.transform.position.x + bgWidth / 2f <= transform.position.x + _screenWidth / 2f)
        {
            obj.transform.position = new Vector3(obj.transform.position.x + bgWidth / 3f, obj.transform.position.y, obj.transform.position.z);
        }
        else if (obj.transform.position.x - bgWidth / 2f >= transform.position.x - _screenWidth / 2f)
        {
            obj.transform.position = new Vector3(obj.transform.position.x - bgWidth / 3f, obj.transform.position.y, obj.transform.position.z);
        }
    }
    
    #endregion
}