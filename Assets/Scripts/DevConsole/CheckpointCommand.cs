using System.IO;
using UnityEngine;

namespace DevConsole
{
    /// <summary>
    /// Command to list checkpoints or set player checkpoint position.
    /// </summary>
    public class CheckpointCommand : IDevCommand
    {
        /// <inheritdoc />
        public string Name => "checkpoint";

        /// <inheritdoc />
        public void Run(string[] args, StringWriter sw)
        {
            if (args.Length is < 1 or > 2)
            {
                PrintUsage(sw);
                return;
            }

            if (args.Length == 1 && args[0].ToLower() is "list" or "l")
            {
                Checkpoint[] checkpoints = Object.FindObjectsOfType<Checkpoint>();

                for (int i = 0; i < checkpoints.Length; i++)
                {
                    sw.WriteLine($"Checkpoint {i} ({checkpoints[i].name}): {checkpoints[i].transform.position}");
                }

                return;
            }

            bool result = IDevCommand.TryGetPosOrCheckpointPos(args, sw, out Vector2 pos);
            if (!result) return;
            GameManager.Instance.CheckPointPos = pos;
        }

        /// <summary>
        /// Prints all usages to the provided StringWriter.
        /// </summary>
        /// <param name="sw">StringWriter, typically provided by the dev console</param>
        private void PrintUsage(StringWriter sw)
        {
            sw.WriteLine(IDevCommand.Color($"Usage: {Name} <checkpoint no>", "red"));
            sw.WriteLine(IDevCommand.Color($"       {Name} <list/l>", "red"));
            sw.WriteLine(IDevCommand.Color($"       {Name} <x> <y>", "red"));
        }
    }
}