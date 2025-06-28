using Managers;
using Player;
using UnityEngine;

namespace Interactables
{
    /// <summary>
    ///     Invisible checkpoint that triggers cutscene immediately after collision;
    ///     after cutscene completion, triggers normal checkpoint stuff
    /// </summary>
    public class CutsceneCheckpoint : Checkpoint
    {
        /// <inheritdoc cref="AbstractPlayerInteractable.OnPlayerEnter"/>
        public override void OnPlayerEnter(PlayerController player)
        {
            StartConversation();
        }
        
        public override void StartConversation()
        {
            if (conversationStartNode == "") return;
            Debug.Log("Started dialogue at checkpoint.");
            IsCurrentConversation = true;
            DialogRunner.StartDialogue(conversationStartNode);
        }
    }
}
