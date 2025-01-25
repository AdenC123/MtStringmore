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
                ("Tutorial_Player_Jump", 1, "Tutorial_Key_Space", 1)},
            {"dash", new Tuple<string, float, string, float>
                ("Tutorial_Player_Dash", 1, "Tutorial_Key_Space", 1)},
            {"swing", new Tuple<string, float, string, float>
                ("Tutorial_Player_Swing", 1, "Tutorial_Key_Space", 1)}
        };
    
    private void OnEnable()
    {
        moveAnim.Play(_moves[_moveToShow].Item1);
        moveAnim.speed = _moves[_moveToShow].Item2;
        moveAnim.Play(_moves[_moveToShow].Item1);
        moveAnim.speed = _moves[_moveToShow].Item4;
    }

    public void SetMove(string move)
    {
        if (_moves.ContainsKey(move))
            _moveToShow = move;
    }
}
