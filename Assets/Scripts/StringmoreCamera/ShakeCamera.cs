using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StringmoreCamera
{
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
        /// <param name = "xShake/yShake">biases the direction in which the camera should shake</param>
        public void Shake(float shakeDuration, float shakeIntensity, bool xShake, bool yShake) 
        {
            if(_activeShake != null) 
            {
                transform.localPosition = _originalCameraPos;
            }
            _activeShake = StartCoroutine(ShakeRoutine(shakeDuration,shakeIntensity,xShake,yShake));
        }

        //corroutine to shake the camera
        private IEnumerator ShakeRoutine(float shakeDuration, float shakeIntensity, bool xShake, bool yShake)
        {
            Vector2 _shakeOffset;
            for (float _elapsed = 0; _elapsed < shakeDuration; _elapsed += Time.deltaTime)  
            {
                Vector2 shakeOffset = Vector2.zero;
                float _currentIntensity = Mathf.Lerp(shakeIntensity, 0f, _elapsed / shakeDuration); //decrease the shake over time

                //perlin noise used for smooth randomness
                if (xShake) shakeOffset.x = (Mathf.PerlinNoise1D(Time.time * 10f) * 2 - 1) * _currentIntensity;
                if (yShake) shakeOffset.y = (Mathf.PerlinNoise(Time.time * 10f, 1) * 2 - 1) * _currentIntensity;

                transform.localPosition = _originalCameraPos + shakeOffset;
                yield return null;
            }
            transform.localPosition = _originalCameraPos;
            _activeShake = null;
            
        }

    }
}