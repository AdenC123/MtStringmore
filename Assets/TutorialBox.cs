using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBox : MonoBehaviour
{
    private string _moveToShow;
    private Animator _moveAnim;
    private Animator _keyAnim;
    private SpriteRenderer _moveSprite;
    private SpriteRenderer _keySprite;
    private Coroutine _fadeCoroutine;

    [SerializeField] private GameObject moveDisplay;
    [SerializeField] private GameObject keyDisplay;
    [SerializeField] private float fadeDuration = 1.0f;
    
    // Dictionary of tutorial moves
    // Tuple items:
    // - Player animation name
    // - Player animation speed
    // - Keyboard animation name
    // - Keyboard animation speed
    private readonly Dictionary<string, Tuple<string, float, string, float>> _moves =
        new()
        {
            {"jump", new Tuple<string, float, string, float>
                ("Tutorial_Player_Jump", 0.25f, "Tutorial_Key_Space", 0.25f)},
            {"dash", new Tuple<string, float, string, float>
                ("Tutorial_Player_Dash", 0.25f, "Tutorial_Key_Space", 0.25f)},
            {"swing", new Tuple<string, float, string, float>
                ("Tutorial_Player_Swing", 0.25f, "Tutorial_Key_Space", 0.25f)}
        };

    private void Start()
    {
        _moveAnim = moveDisplay.GetComponent<Animator>();
        _moveSprite = moveDisplay.GetComponent<SpriteRenderer>();
        _keyAnim = keyDisplay.GetComponent<Animator>();
        _keySprite = keyDisplay.GetComponent<SpriteRenderer>();
    }

    public void SetMove(string move)
    {
        if (_moves.ContainsKey(move))
            _moveToShow = move;
        else
            Debug.LogError("Invalid tutorial move set");
    }
    
    public void ShowTutorial()
    {
        _moveAnim.enabled = true;
        _moveAnim.Play(_moves[_moveToShow].Item1);
        _moveAnim.speed = _moves[_moveToShow].Item2;
        
        _keyAnim.enabled = true;
        _keyAnim.Play(_moves[_moveToShow].Item3);
        _keyAnim.speed = _moves[_moveToShow].Item4;

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }
        _fadeCoroutine = StartCoroutine(FadeIn());
    }

    public void HideTutorial()
    {
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }
        _fadeCoroutine = StartCoroutine(FadeOut());
        
        _moveAnim.enabled = false;
        _keyAnim.enabled = false;
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
    }
}
