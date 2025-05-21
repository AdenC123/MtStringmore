using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;

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
    /// <param>
    /// shakeDuration: the length of time to shake the camera for
    /// shakeIntensity: the level of intensity to shake the camera (higher values cause more aggressive/violent shaking)
    /// shakeDecay: how quickly the camera shake dissipates (large values cause shake to quickly dissapate)
    /// xShake/yShake: biases the direction in which the camera should shake for
    /// </param>
    public void Shake(float shakeDuration, float shakeIntensity, float shakeDecay, bool xShake, bool yShake) 
    {
        //TODO: trying to prevent coroutines from overlapping need to fix
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
        while (_elapsed < shakeDuration)  
        {
            if(xShake && yShake)    //shake camera both directions
            {
                //perlin noise used for smooth randomness
                _shakeOffset = new Vector2(
                    Mathf.PerlinNoise(Time.time * 10f, 0) * 2 - 1,  
                    Mathf.PerlinNoise(Time.time * 10f, 1) * 2 - 1   
                ) * shakeIntensity;
            } else if(xShake)   //shake camera only in x direction
            {
                _shakeOffset = new Vector2(Mathf.PerlinNoise(Time.time * 10f, 0) * 2 - 1, 0) * shakeIntensity;
            } else  //shake camera only in y direction
            {
                _shakeOffset = new Vector2(0,Mathf.PerlinNoise(Time.time * 10f, 1) * 2 - 1 ) * shakeIntensity;
            }

            // lerp between the original camera position and the shake offset
            transform.localPosition = Vector2.Lerp(
                transform.localPosition,                            // start pos
                _originalCameraPos + _shakeOffset,                   // target pos
                shakeDecay * Time.deltaTime                        // reduce shaking over time
            );

            _elapsed += Time.deltaTime;
            //yield return new WaitForSeconds(0.01f); //TODO this value might need tweaking based on performance issues
            yield return null;
        }

        transform.localPosition = _originalCameraPos;
        _activeShake = null;
        
    }

}
