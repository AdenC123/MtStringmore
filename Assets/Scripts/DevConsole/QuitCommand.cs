﻿using System.IO;
using UnityEngine;

namespace DevConsole
{
    /// <summary>
    /// Command to quit the game.
    /// </summary>
    public class QuitCommand : IDevCommand
    {
        /// <inheritdoc />
        public bool RequiresCheats => false;

        /// <inheritdoc />
        public string Name => "quit";
        
        /// <inheritdoc />
        public void Run(string[] args, StringWriter sw)
        {
            Application.Quit();
        }
    }
}