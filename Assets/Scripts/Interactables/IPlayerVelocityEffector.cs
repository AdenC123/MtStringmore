﻿using System;
using UnityEngine;

namespace Interactables
{
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
        /// Whether this effector ignores other effectors (i.e. other effectors can't override this one).
        /// </summary>
        bool IgnoreOtherEffectors => true;

        /// <summary>
        /// Gets the desired velocity of the player after applying velocity effects.
        /// </summary>
        /// <param name="velocity">Original player velocity</param>
        /// <returns>Affected velocity</returns>
        Vector2 ApplyVelocity(Vector2 velocity);
    }

    /// <summary>
    /// Simple velocity effector that takes in an input function.
    /// </summary>
    public class SimpleVelocityEffector : IPlayerVelocityEffector
    {
        private readonly Func<Vector2, Vector2> _velocityFunction;

        public SimpleVelocityEffector(Func<Vector2, Vector2> velocityFunction)
        {
            _velocityFunction = velocityFunction;
        }

        public Vector2 ApplyVelocity(Vector2 velocity)
        {
            return _velocityFunction(velocity);
        }
    }
}