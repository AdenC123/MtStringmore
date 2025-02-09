using System.IO;
using UnityEngine;

namespace DevConsole
{
    /// <summary>
    /// Command to list checkpoints or set player checkpoint position.
    /// </summary>
    public class CheckpointCommand : IDevCommand
    {
        public string Name => "checkpoint";
        
        public void Run(string[] args, StringWriter sw)
        {
            switch (args.Length)
            {
                case 1:
                {
                    // yes, if the guy types it in wrong it incurs a performance hit
                    // just don't type it in wrong 5head
                    Checkpoint[] checkpoints = Object.FindObjectsOfType<Checkpoint>();
                    if (args[0].ToLower() == "list" || args[0].ToLower() == "l")
                    {
                        for (int i = 0; i < checkpoints.Length; i++)
                        {
                            sw.WriteLine($"Checkpoint {i} ({checkpoints[i].name}): {checkpoints[i].transform.position}");
                        }

                        return;
                    }
                    if (!int.TryParse(args[0], out int checkpointNo))
                    {
                        PrintUsage(sw);
                        return;
                    }
                    if (checkpointNo < 0 || checkpointNo >= checkpoints.Length)
                    {
                        sw.WriteLine(IDevCommand.Color($"Invalid checkpoint number: must be in range [0,{checkpoints.Length})", "red"));
                        return;
                    }
                    
                    GameManager.Instance.CheckPointPos = checkpoints[checkpointNo].transform.position;
                    return;
                }
                case 2:
                {
                    if (!float.TryParse(args[0], out float x) || !float.TryParse(args[1], out float y))
                    {
                        PrintUsage(sw);
                        return;
                    }
                    GameManager.Instance.CheckPointPos = new Vector2(x, y);
                    return;
                }
                default:
                    PrintUsage(sw);
                    return;
            }
        }

        private void PrintUsage(StringWriter sw)
        {
            sw.WriteLine(IDevCommand.Color($"Usage: {Name} <checkpoint no>", "red"));
            sw.WriteLine(IDevCommand.Color($"       {Name} <list/l>", "red"));
            sw.WriteLine(IDevCommand.Color($"       {Name} <x> <y>", "red"));
        }
    }
}