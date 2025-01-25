using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBox : MonoBehaviour
{
    private string _moveToShow;
    [SerializeField] private Animator moveAnim;
    [SerializeField] private Animator keyAnim;
    
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
    
    public void SetMove(string move)
    {
        if (_moves.ContainsKey(move))
            _moveToShow = move;
        else
            Debug.LogError("Invalid tutorial move set");
    }
    
    public void ShowTutorial()
    {
        moveAnim.enabled = true;
        keyAnim.enabled = true;
        moveAnim.Play(_moves[_moveToShow].Item1);
        moveAnim.speed = _moves[_moveToShow].Item2;
        keyAnim.Play(_moves[_moveToShow].Item3);
        keyAnim.speed = _moves[_moveToShow].Item4;
    }

    public void HideTutorial()
    {
        moveAnim.enabled = false;
        keyAnim.enabled = false;
    }
}
