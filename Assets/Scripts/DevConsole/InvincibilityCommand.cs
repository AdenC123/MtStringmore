using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DevConsole
{
    /// <summary>
    /// Toggles player invincibility.
    /// </summary>
    public class InvincibilityCommand : IDevCommand
    {
        private bool _isInvincible;
        
        public string Name => "sv_invincibility";

        public InvincibilityCommand()
        {
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
        }

        /// <summary>
        /// Listener on scene changes to set player invincibility on scene load.
        /// </summary>
        /// <param name="curr">Current scene (ignored)</param>
        /// <param name="next">Next scene (ignored)</param>
        /// <remarks>
        /// There's probably a better way to do this but I cba.
        /// </remarks>
        private void SceneManagerOnActiveSceneChanged(Scene curr, Scene next)
        {
            if (!_isInvincible) return;
            PlayerController pc = Object.FindObjectOfType<PlayerController>();
            if (!pc)
            {
                // ideally I'd like to access a StringWriter but hey i mean i hooked Debug Log so that works
                Debug.LogError("Player not found.");
                return;
            }

            pc.DebugIgnoreDeath = _isInvincible;
        }

        public void Run(string[] args, StringWriter sw)
        {
            if (args.Length != 1 || !IDevCommand.TryParseBool(args[0], out bool arg))
            {
                sw.WriteLine(IDevCommand.Color($"Usage: {Name} <1/0>", "red"));
                return;
            }
            
            PlayerController pc = Object.FindObjectOfType<PlayerController>();
            if (!pc)
            {
                sw.WriteLine(IDevCommand.Color("Player not found.", "red"));
                return;
            }

            _isInvincible = arg;
            pc.DebugIgnoreDeath = _isInvincible;
        }
    }
}