using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Loads background layers from child objects, and moves them with respect to camera movement
/// </summary>
public class ParallaxBackground : MonoBehaviour
{
    private ParallaxCamera _parallaxCamera;
    private readonly List<ParallaxLayer> _parallaxLayers = new();

    private void Start()
    {
        _parallaxCamera = Camera.main?.GetComponent<ParallaxCamera>();

        if (_parallaxCamera is not null)
            _parallaxCamera.Moved += Move;

        SetLayers();
    }

    private void SetLayers()
    {
        _parallaxLayers.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            if (!transform.GetChild(i).TryGetComponent(out ParallaxLayer layer)) continue;
            layer.name = "Layer-" + i;
            _parallaxLayers.Add(layer);
        }
    }

    private void Move(float deltaX, float deltaY)
    {
        foreach (var layer in _parallaxLayers)
        {
            layer.Move(deltaX, deltaY);
        }
    }
}