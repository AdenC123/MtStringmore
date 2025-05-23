using System.Linq;
using UnityEngine;

namespace Util
{
    /// <summary>
    /// Input utility class.
    /// </summary>
    public static class InputUtil
    {
        /// <summary>
        /// Returns true if the player starts the jump button or there's any new touches in the screen.
        /// </summary>
        /// <returns>True if player started to jump (press space bar/touch screen)</returns>
        public static bool StartJumpOrTouch()
        {
            return Input.GetButtonDown("Jump") || Input.touches.Any(touch => touch.phase == TouchPhase.Began);
        }

        /// <summary>
        /// Returns true if player's holding the jump action (i.e. holding space or holding touches).
        /// </summary>
        /// <returns>True if player holds the jump action</returns>
        public static bool HoldJumpOrTouch()
        {
            return Input.GetButton("Jump") || Input.touchCount > 0;
        }
    }
}
