using System;
using UnityEngine;

/// <summary>
/// Fixes the camera at a set position when the player enters the bounding box.
/// Camera position is the centre of the box collider, plus an offset.
/// Uses "MainCamera" and "Player" tags.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class FixCameraTrigger : MonoBehaviour
{
    [SerializeField] private Vector2 targetOffset;
    [Obsolete("Please use fixTypeX"), Tooltip("Obsolete: please use fixTypeX")]
    public bool fixX = true;

    public FixCameraType fixTypeX = FixCameraType.Invalid;

    [Obsolete("Please use fixTypeY"), Tooltip("Obsolete: please use fixTypeY")]  
    public bool fixY = true;
    
    public FixCameraType fixTypeY = FixCameraType.Invalid;

    private Vector2 _bound;
    private FollowCamera _cam;

    private void Awake()
    {
        _cam = GameObject.FindWithTag("MainCamera").GetComponent<FollowCamera>();
        Vector2 boxOffset = GetComponent<BoxCollider2D>().offset;
        _bound = (Vector2)transform.position + boxOffset + targetOffset;
        SetValidFixType();
    }

    
#pragma warning disable CS0618 // Type or member is obsolete

    private void SetValidFixType()
    {
        if (fixTypeX == FixCameraType.Invalid)
        {
            fixTypeX = fixX ? FixCameraType.AllowEqual : FixCameraType.None;
            Debug.LogWarning($"Invalid Camera FixType - importing from {fixX}");
        }
        if (fixTypeY == FixCameraType.Invalid)
        {
            fixTypeY = fixY ? FixCameraType.AllowEqual : FixCameraType.None;
            Debug.LogWarning($"Invalid Camera FixType - importing from {fixY}");
        }
    }
    private void OnValidate()
    {
        SetValidFixType();
    }
#pragma warning restore CS0618 // Type or member is obsolete

    private static float AffectField(float original, float target, FixCameraType fixType)
    {
        return fixType switch
        {
            FixCameraType.AllowEqual => target,
            FixCameraType.None => original,
            FixCameraType.AllowGreater => Mathf.Min(original, target),
            FixCameraType.AllowLess => Mathf.Max(original, target),
            _ => original
        };
    }
    
    public Vector2 AffectTarget(Vector2 target)
    {
        target.x = AffectField(target.x, _bound.x, fixTypeX);
        target.y = AffectField(target.y, _bound.y, fixTypeY);
        return target;
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

    public enum FixCameraType
    {
        Invalid, AllowEqual, AllowLess, AllowGreater, None
    }
}
