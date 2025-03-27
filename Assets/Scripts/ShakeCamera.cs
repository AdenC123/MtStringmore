using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;

public class ShakeCamera : MonoBehaviour
{   
     #region Serialized Private Fields
    [Header("Shakey-Shakey")]
    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeIntensity;
    [SerializeField] private float shakeDecay;      //rapid or slow decay overtime
    #endregion

    #region Private Fields
    private PlayerController _playerControl;
    private IEnumerator _activeRoutine;
    #endregion

    void Start()
    {
        GameObject _player = GameObject.FindWithTag("Player");
        _playerControl=_player.GetComponent<PlayerController>();
    }
    void Update()
    {
        if(_playerControl.PlayerState == PlayerStateEnum.Dash) 
        {
            //Debug.Log("We dashin");
            StartCoroutine(Shake()); 
        }  
    }

    IEnumerator Shake()
    {
        // Debug.Log("We shakin");
        Vector2 _originalCameraPos = transform.localPosition;  
        float _elapsed = 0.0f;  
        Debug.Log(_originalCameraPos);
        while (_elapsed < shakeDuration)  
        {
            //perlin noise used for smooth randomness
            Vector2 shakeOffset = new Vector2(
                Mathf.PerlinNoise(Time.time * 10f, 0) * 2 - 1,  
                Mathf.PerlinNoise(Time.time * 10f, 1) * 2 - 1   
            ) * shakeIntensity;

            // lerp between the original camera position and the shake offset
            transform.localPosition = Vector2.Lerp(
                transform.localPosition,                            // start pos
                _originalCameraPos + shakeOffset,                   // target pos
                shakeDecay * Time.deltaTime                        // reduce shaking over time
            );

            _elapsed += Time.deltaTime;

            yield return null;
        }

    
        transform.localPosition = _originalCameraPos;
    }

}
