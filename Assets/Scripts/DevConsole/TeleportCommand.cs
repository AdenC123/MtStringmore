using System.IO;
using UnityEngine;

namespace DevConsole
{
    /// <summary>
    /// Command for teleportation to a specific position.
    /// </summary>
    public class TeleportCommand : IDevCommand
    {
        public string Name => "tp";
        
        public void Run(string[] args, StringWriter sw)
        {
            if (args.Length != 2 || !float.TryParse(args[0], out float x) || !float.TryParse(args[1], out float y))
            {
                sw.WriteLine(IDevCommand.Color($"Usage: {Name} <x> <y>", "red"));
                return;
            }
            
            PlayerController pc = Object.FindObjectOfType<PlayerController>();
            if (!pc)
            {
                sw.WriteLine(IDevCommand.Color("Player not found.", "red"));
                return;
            }
            
            pc.gameObject.transform.position = new Vector3(x, y, 0);
        }
    }
}