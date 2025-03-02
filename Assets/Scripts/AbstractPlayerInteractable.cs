using System;
using UnityEngine;

/// <summary>
/// Abstract player interactable: detects if the player walks into the trigger, and if the player presses space,
/// <see cref="PlayerController"/> would call <see cref="StartInteract"/> then <see cref="EndInteract"/> when the player
/// lets go.
/// </summary>
/// <remarks>
/// TODO possibly refactor swing to use this just like how i did it with trampoline/bouncy platform
/// </remarks>
[DisallowMultipleComponent, RequireComponent(typeof(Collider2D))]
public abstract class AbstractPlayerInteractable : MonoBehaviour, IPlayerVelocityEffector
{
    /// <inheritdoc />
    public virtual bool IgnoreGravity => false;

    /// <inheritdoc />
    public virtual bool IgnoreOtherEffectors => true;

    /// <summary>
    /// Whether to disallow re-interaction (by simulating player exit without actually leaving the trigger).
    /// </summary>
    public virtual bool DisallowReinteraction => true;

    /// <summary>
    /// Event triggered on player enter.
    /// </summary>
    /// <remarks>
    /// Currently unused, but may come in handy later.
    /// </remarks>
    public event Action<PlayerController> OnPlayerEnter;

    /// <summary>
    /// Event triggered on player exit.
    /// </summary>
    /// <remarks>
    /// Currently unused, but may come in handy later.
    /// </remarks>
    public event Action<PlayerController> OnPlayerExit;

    /// <inheritdoc />
    public abstract Vector2 ApplyVelocity(Vector2 velocity);

    /// <summary>
    /// Called by <see cref="PlayerController"/> when the player starts interacting with this object.
    /// </summary>
    /// <param name="player">Interacting player</param>
    public abstract void StartInteract(PlayerController player);

    /// <summary>
    /// Called by <see cref="PlayerController"/> when the player stops interacting with this object.
    /// </summary>
    /// <param name="player">Interacting player</param>
    public abstract void EndInteract(PlayerController player);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent(out PlayerController player)) return;
        player.CurrentInteractableArea = this;
        OnPlayerEnter?.Invoke(player);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent(out PlayerController player)) return;
        if (player.CurrentInteractableArea != this)
        {
            Debug.LogWarning("Player's current interactable area does not match!");
        }
        else if (DisallowReinteraction)
        {
            player.CurrentInteractableArea = null;
        }

        OnPlayerExit?.Invoke(player);
    }
}
