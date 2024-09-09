using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Trigger attached to a game object that plays dialog when the player
/// collides with it.
/// </summary>
public class DialogTrigger : MonoBehaviour
{
    public Dialog dialog;

    /// <summary>
    /// Start sequence of dialog messages set in this dialog trigger.
    /// </summary>
    public void TriggerDialog()
    {
        var dialogManager = GameManager.Instance.DialogManager;
        dialogManager.StartDialog(dialog);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerDialog();
        }
    }
}

/// <summary>
/// Line to be displayed in one dialog box, and the icon of the character that speaks it.
/// </summary>
[System.Serializable]
public class DialogLine
{
    public Sprite icon;
    [TextArea(3, 10)] public string line;
}

/// <summary>
/// List of <see cref="DialogLine"/> to be displayed in one sequence.
/// </summary>
[System.Serializable]
public class Dialog
{
    public List<DialogLine> dialogLines = new();
}
