using UnityEngine;

/// <summary>
/// Object that modifies player velocity (e.g. a trampoline or bouncy platform).
/// </summary>
public interface IPlayerVelocityEffector
{
    /// <summary>
    /// Whether the player skips gravity logic.
    ///
    /// Useful if you're applying upward motion in a trigger area.
    /// </summary>
    bool IgnoreGravity => false;

    /// <summary>
    /// Gets the desired velocity of the player after applying velocity effects.
    /// </summary>
    /// <param name="velocity">Original player velocity</param>
    /// <returns>Affected velocity</returns>
    Vector2 ApplyVelocity(Vector2 velocity);
}
