using Managers;
using Player;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

namespace Interactables
{
    /// <summary>
    ///     Checkpoint flag that sets checkpoint position when player collides with it
    /// </summary>
    public class Checkpoint : AbstractPlayerInteractable
    {
        private static readonly int HoistKey = Animator.StringToHash("Hoisted");

        [Header("References")] [SerializeField]
        private Animator anim;

        [SerializeField, Tooltip("Starts conversations on hit bypassing testing and final checkpoint checks")]
        private bool toQuoteReactDO_NOT_USE_OR_ELSE_YOU_WILL_BE_FIRED;

        [SerializeField] private SpriteRenderer sprite;

        [Tooltip("Node that starts from this checkpoint. Set to \"\" to not trigger dialog from checkpoint.")]
        [SerializeField]
        private string conversationStartNode;

        [Tooltip("If checked, the player faces left when they respawn on this checkpoint")] [SerializeField]
        private bool respawnFacingLeft;

        [SerializeField] private Vector2 spawnOffset;
        public UnityEvent onCheckpointReached;
        
        /// <summary>
        /// Whether this checkpoint has a conversation.
        /// </summary>
        public bool HasConversation => !string.IsNullOrWhiteSpace(conversationStartNode);

        // internal properties not exposed to editor
        private DialogueRunner _dialogueRunner;
        private bool _isCurrentConversation;

        public bool hasBeenHit;

        public void Start()
        {
            hasBeenHit = false;
            _dialogueRunner = FindObjectOfType<DialogueRunner>();
            if (_dialogueRunner) _dialogueRunner.onDialogueComplete.AddListener(EndConversation);
        }

        /// <summary>
        /// Called on checkpoint hit.
        /// </summary>
        private void HitCheckpoint()
        {
            if (hasBeenHit) return;
            hasBeenHit = true;
            onCheckpointReached.Invoke();
            if (toQuoteReactDO_NOT_USE_OR_ELSE_YOU_WILL_BE_FIRED)
            {
                StartConversation();
            }
        }

        /// <inheritdoc cref="AbstractPlayerInteractable.OnPlayerEnter"/>
        public override void OnPlayerEnter(PlayerController player)
        {
            if (!anim.GetBool(HoistKey))
            {
                HitCheckpoint();
                anim.SetBool(HoistKey, true);
                GameManager.Instance.UpdateCheckpointData(transform.position + (Vector3)spawnOffset,
                    respawnFacingLeft);
            }

            float signX = respawnFacingLeft ? -1 : 1;
            if (player.Direction * signX > 0)
            {
                // disallow flipping if going right direction
                player.CurrentInteractableArea = null;
            }
        }

        /// <summary>
        /// Flips the checkpoint respawn and resets it.
        /// </summary>
        public void FlipAndResetCheckpoint()
        {
            respawnFacingLeft = !respawnFacingLeft;
            anim.SetBool(HoistKey, false);
            hasBeenHit = false;
        }

        /// <summary>
        /// Starts the conversation at <see cref="conversationStartNode"/> unless it's blank.
        /// </summary>
        public void StartConversation()
        {
            if (string.IsNullOrWhiteSpace(conversationStartNode)) return;
            Debug.Log("Started dialogue at checkpoint.");
            _isCurrentConversation = true;
            _dialogueRunner.StartDialogue(conversationStartNode);
            if (!toQuoteReactDO_NOT_USE_OR_ELSE_YOU_WILL_BE_FIRED)
                Time.timeScale = 0;
        }

        /// <summary>
        /// Ends the conversation if active.
        /// </summary>
        private void EndConversation()
        {
            if (!_isCurrentConversation) return;
            _isCurrentConversation = false;
            Debug.Log("Ended dialogue at checkpoint.");
            Time.timeScale = 1;
        }

        /// <inheritdoc cref="AbstractPlayerInteractable.OnPlayerExit"/>
        public override void OnPlayerExit(PlayerController player)
        {
        }

        /// <summary>
        /// Flips the player if they're going the wrong direction.
        /// </summary>
        /// <param name="velocity">Player's velocity</param>
        /// <returns>New velocity</returns>
        public override Vector2 ApplyVelocity(Vector2 velocity)
        {
            float signX = respawnFacingLeft ? -1 : 1;
            return Mathf.Sign(velocity.x * signX) < 0 ? new Vector2(signX, velocity.y) : velocity;
        }

        /// <inheritdoc cref="AbstractPlayerInteractable.StartInteract"/>
        public override void StartInteract(PlayerController player)
        {
            player.AddPlayerVelocityEffector(this, true);
            player.StopInteraction(this);
            player.CurrentInteractableArea = null;
        }

        /// <inheritdoc cref="AbstractPlayerInteractable.EndInteract"/>
        public override void EndInteract(PlayerController player)
        {
        }
    }
}
