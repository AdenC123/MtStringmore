using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Adds a rumbling shake to the MainCamera
/// </summary>
public class ShakeCamera : MonoBehaviour
{   
    //TODO: possibly convert class to be a "camera effector" with more methods to cause different camera effects (shakes,zooms in/out etc)
    private Coroutine _activeShake;
    private Vector2 _originalCameraPos;

    private void Awake()
    {
        _originalCameraPos = transform.localPosition;
    }
    /// <summary>
    /// Public method that any class can shake the camera.
    /// </summary>
    /// <param name = "shakeDuration">the length of time to shake the camera for</param>
    /// <param name = "shakeIntensity">the level of intensity to shake the camera (higher values cause more aggressive/violent shaking)</param>
    /// <param name = "shakeDecay">how quickly the camera shake dissipates (large values cause shake to quickly dissapate)</param>
    /// <param name = "xShake/yShake">biases the direction in which the camera should shake for</param>
    public void Shake(float shakeDuration, float shakeIntensity, float shakeDecay, bool xShake, bool yShake) 
    {
        if(_activeShake != null) 
        {
            transform.localPosition = _originalCameraPos;
        }
        _activeShake = StartCoroutine(ShakeRoutine(shakeDuration,shakeIntensity,shakeDecay,xShake,yShake));
    }

    //corroutine to shake the camera
    private IEnumerator ShakeRoutine(float shakeDuration, float shakeIntensity, float shakeDecay, bool xShake, bool yShake)
    {
        float _elapsed = 0.0f;  
        Vector2 _shakeOffset;
        for (float elapsed = 0; elapsed < shakeDuration; elapsed += Time.deltaTime)  
        {
            Vector2 shakeOffset = Vector2.zero;
            //perlin noise used for smooth randomness
            if (xShake) shakeOffset.x = (Mathf.PerlinNoise1D(Time.time * 10f) * 2 - 1) * shakeIntensity;
            if (yShake) shakeOffset.y = (Mathf.PerlinNoise(Time.time * 10f, 1) * 2 - 1) * shakeIntensity;

            // lerp between the original camera position and the shake offset
            transform.localPosition = Vector2.Lerp(
                transform.localPosition,                            // start pos
                _originalCameraPos + shakeOffset,                   // target pos
                shakeDecay * Time.deltaTime                        // reduce shaking over time
            );
            yield return null;
        }
        transform.localPosition = _originalCameraPos;
        _activeShake = null;
        
    }

}
