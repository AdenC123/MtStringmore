using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Receives dialogs to be displayed and handles display of the dialogs in the UI.
/// </summary>
public class DialogManager : MonoBehaviour
{
    public Image characterIcon;
    public TextMeshProUGUI dialogText;
    public Canvas dialogTextbox;

    [NonSerialized] private Queue<DialogLine> _lines = new();
    [NonSerialized] public bool IsDialogActive = false;
    public float typingSpeed = 0.2f;

    /// <summary>
    /// Start displaying a dialog sequence.
    /// </summary>
    /// <param name="dialog">Sequence of dialog messages to be displayed.</param>
    public void StartDialog(Dialog dialog)
    {
        dialogTextbox.gameObject.SetActive(true);
        IsDialogActive = true;
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
        characterIcon.sprite = currentLine.icon;
        
        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine));
    }

    IEnumerator TypeSentence(DialogLine dialogLine)
    {
        dialogText.text = "";
        foreach (char letter in dialogLine.line)
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    void EndDialog()
    {
        IsDialogActive = false;
        dialogTextbox.gameObject.SetActive(true);
    }
}
