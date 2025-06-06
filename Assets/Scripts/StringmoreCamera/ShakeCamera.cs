using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace StringmoreCamera
{
    /// <summary>
    /// Adds a rumbling shake to the MainCamera
    /// </summary>
    public class ShakeCamera : MonoBehaviour
    {
        //private Vector2 _originalCameraPos;
        private FollowCamera _followCamera;
        private Coroutine _activeDashShake;
        private Coroutine _activeBreakingShake;

        private void Awake()
        {
            _followCamera = FindObjectOfType<FollowCamera>();
            _activeDashShake = null;
            _activeBreakingShake = null;
        }

        /// <summary>
        /// Public method that any class can shake the camera.
        /// </summary>
        /// <param name = "shakeDuration">the length of time to shake the camera for</param>
        /// <param name = "shakeIntensity">the level of intensity to shake the camera (higher values cause more aggressive/violent shaking)</param>
        /// <param name = "xShake">biases the x direction in which the camera should shake</param>
        /// <param name = "yShake">biases the y direction in which the camera should shake</param>
        /// <param name = "breakable">set to true if object is breakable</param>
        public void Shake(float shakeDuration, float shakeIntensity, bool xShake, bool yShake, bool breakable)
        {
            // Stop previous shakes of the same type
            if (breakable && _activeBreakingShake != null)
            {
                StopCoroutine(_activeBreakingShake);
            }
            else if (!breakable && _activeDashShake != null)
            {
                StopCoroutine(_activeDashShake);
            }
            
                

            // Start new shake
            var coroutine = StartCoroutine(ShakeRoutine(shakeDuration, shakeIntensity, xShake, yShake));
            if (breakable)
            {
                _activeBreakingShake = coroutine;
            }
            else
            {
                _activeDashShake = coroutine;
            }
            
                
        }

        //corroutine to shake the camera
        private IEnumerator ShakeRoutine(float shakeDuration, float shakeIntensity, bool xShake, bool yShake)
        {
            for (float _elapsed = 0; _elapsed < shakeDuration; _elapsed += Time.deltaTime)
            {
                float _currentIntensity =
                    shakeIntensity * (1f - (_elapsed / shakeDuration)); //decrease the shake over time

                float noiseTime = Time.time * 10f;
                Vector2 shakeOffset = new Vector2(
                    xShake ? (Mathf.PerlinNoise(noiseTime, 0f) - 0.5f) * 2f * _currentIntensity : 0f,
                    yShake ? (Mathf.PerlinNoise(0f, noiseTime) - 0.5f) * 2f * _currentIntensity : 0f
                );

                _followCamera.ShakeOffset = new Vector3(shakeOffset.x,shakeOffset.y,0);
                yield return null;
            }

            _followCamera.ShakeOffset = Vector3.zero; // Reset after shake

        }
    }
}
