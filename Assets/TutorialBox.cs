using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles showing and hiding tutorial.
// Fades in when a checkpoint flag calls the ShowTutorial function,
// Fades out 3 seconds after the user presses the key associated with the move.
public class TutorialBox : MonoBehaviour
{
    private string _moveToShow;
    private Animator _moveAnim;
    private Animator _keyAnim;
    private SpriteRenderer _moveSprite;
    private SpriteRenderer _keySprite;
    private Coroutine _fadeInCoroutine;
    private Coroutine _fadeOutCoroutine;

    [SerializeField] private GameObject moveDisplay;
    [SerializeField] private GameObject keyDisplay;
    [SerializeField] private float fadeDuration = 0.5f;
    
    // Dictionary of tutorial moves
    // Tuple items:
    // - Player animation name
    // - Player animation speed
    // - Keyboard animation name
    // - Keyboard animation speed
    // - Key to press
    private readonly Dictionary<string, Tuple<string, float, string, float, KeyCode>> _moves =
        new()
        {
            {"jump", new Tuple<string, float, string, float, KeyCode>
                ("Tutorial_Player_Jump", 0.25f, "Tutorial_Key_Space", 0.25f, KeyCode.Space)},
            {"dash", new Tuple<string, float, string, float, KeyCode>
                ("Tutorial_Player_Dash", 0.25f, "Tutorial_Key_Space", 0.25f, KeyCode.Space)},
            {"swing_enter", new Tuple<string, float, string, float, KeyCode>
                ("Tutorial_Player_Swing_Enter", 0.25f, "Tutorial_Key_Space", 0.25f, KeyCode.Space)},
            {"swing_leave", new Tuple<string, float, string, float, KeyCode>
                ("Tutorial_Player_Swing_Leave", 0.25f, "Tutorial_Key_Space_Rev", 0.25f, KeyCode.Space)}
        };

    private void Start()
    {
        _moveAnim = moveDisplay.GetComponent<Animator>();
        _moveSprite = moveDisplay.GetComponent<SpriteRenderer>();
        _keyAnim = keyDisplay.GetComponent<Animator>();
        _keySprite = keyDisplay.GetComponent<SpriteRenderer>();
    }

    public void ShowTutorial(string move)
    {
        if (_moves.ContainsKey(move))
            _moveToShow = move;
        else
        {
            Debug.LogError("Invalid tutorial move set");
            return;
        }
        
        _moveAnim.enabled = true;
        _moveAnim.Play(_moves[_moveToShow].Item1);
        _moveAnim.speed = _moves[_moveToShow].Item2;
        
        _keyAnim.enabled = true;
        _keyAnim.Play(_moves[_moveToShow].Item3);
        _keyAnim.speed = _moves[_moveToShow].Item4;

        if (_fadeInCoroutine != null)
            StopCoroutine(_fadeInCoroutine);
        _fadeInCoroutine = StartCoroutine(FadeIn());
    }

    public void HideTutorial()
    {
        if (_fadeOutCoroutine != null)
            StopCoroutine(_fadeOutCoroutine);
        _fadeOutCoroutine = StartCoroutine(FadeOut());
    }
    
    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color moveColor = _moveSprite.color;
        Color keyColor = _keySprite.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            moveColor.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            keyColor.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            _moveSprite.color = moveColor;
            _keySprite.color = keyColor;
            yield return null;
        }
        moveColor.a = 1f;
        keyColor.a = 1f;
        _moveSprite.color = moveColor;
        _keySprite.color = keyColor;
    }

    private IEnumerator FadeOut()
    {
        while (!Mathf.Approximately(_moveSprite.color.a, 1f))
            yield return null;
        
        float elapsedTime = 0f;
        Color moveColor = _moveSprite.color;
        Color keyColor = _keySprite.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            moveColor.a = Mathf.Clamp01(1f - elapsedTime / fadeDuration);
            keyColor.a = Mathf.Clamp01(1f - elapsedTime / fadeDuration);
            _moveSprite.color = moveColor;
            _keySprite.color = keyColor;
            yield return null;
        }

        moveColor.a = 0f;
        keyColor.a = 0f;
        _moveSprite.color = moveColor;
        _keySprite.color = keyColor;
        
        _moveAnim.enabled = false;
        _keyAnim.enabled = false;
    }
}
