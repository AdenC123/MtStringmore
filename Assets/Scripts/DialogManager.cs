using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Receives dialogs to be displayed and handles display of the dialogs in the UI.
/// </summary>
public class DialogManager : MonoBehaviour
{
    public Image characterIcon;

    public TextMeshProUGUI characterName;

    public TextMeshProUGUI dialogArea;

    [NonSerialized] private Queue<DialogLine> _lines;
    [NonSerialized] public bool IsDialogActive = false;
    public float typingSpeed = 0.2f;
    public Animator animator;

    /// <summary>
    /// Start displaying a dialog sequence.
    /// </summary>
    /// <param name="dialog">Sequence of dialog messages to be displayed.</param>
    public void StartDialog(Dialog dialog)
    {
        IsDialogActive = true;
        animator.Play("Show");
        _lines.Clear();
        foreach (DialogLine line in dialog.dialogLines)
        {
            _lines.Enqueue(line);
        }
        ShowNextDialogLine();
    }

    /// <summary>
    /// Show next dialog message.
    /// </summary>
    public void ShowNextDialogLine()
    {
        if (_lines.Count == 0)
        {
            EndDialog();
            return;
        }

        DialogLine currentLine = _lines.Dequeue();
        characterIcon.sprite = currentLine.character.icon;
        characterName.text = currentLine.character.name;
        
        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine));
    }

    IEnumerator TypeSentence(DialogLine dialogLine)
    {
        dialogArea.text = "";
        foreach (char letter in dialogLine.line)
        {
            dialogArea.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    void EndDialog()
    {
        IsDialogActive = false;
        animator.Play("Hide");
    }
}
