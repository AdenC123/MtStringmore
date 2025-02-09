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

            bool result = GetPosition(args, sw, out Vector2 pos);
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

        /// <summary>
        /// Utility to parse position arguments that's either (1) checkpoint num or (2) x/y pair.
        ///
        /// X/Y pair can be relational, e.g. ~+20 is player position +20. ~20 is also accepted, but 20~ is not.
        /// </summary>
        /// <param name="args">Arguments</param>
        /// <param name="sw">StringWriter to output log messages to</param>
        /// <param name="pos">Output position if provided</param>
        /// <returns>Result of parsing: true if successful, false otherwise</returns>
        public static bool GetPosition(string[] args, StringWriter sw, out Vector2 pos)
        {
            pos = default;
            switch (args.Length)
            {
                case 1:
                {
                    if (!int.TryParse(args[0], out int checkpointNo))
                    {
                        sw.WriteLine(IDevCommand.Color($"Invalid checkpoint number: {args[0]}", "red"));
                        return false;
                    }

                    Checkpoint[] checkpoints = Object.FindObjectsOfType<Checkpoint>();
                    if (checkpointNo < 0 || checkpointNo >= checkpoints.Length)
                    {
                        sw.WriteLine(IDevCommand.Color(
                            checkpoints.Length == 0
                                ? "No checkpoints in the scene available to teleport."
                                : $"Invalid checkpoint number: must be in range [0,{checkpoints.Length})", "red"));
                        return false;
                    }

                    pos = checkpoints[checkpointNo].transform.position;
                    return true;
                }
                case 2:
                {
                    if (!args[0].StartsWith('~') && !args[1].StartsWith('~'))
                    {
                        // position is absolute
                        if (!float.TryParse(args[0], out float x))
                        {
                            sw.WriteLine(IDevCommand.Color($"Invalid positional argument: {args[0]}", "red"));
                            return false;
                        }

                        if (!float.TryParse(args[1], out float y))
                        {
                            sw.WriteLine(IDevCommand.Color($"Invalid positional argument: {args[1]}", "red"));
                            return false;
                        }

                        pos = new Vector2(x, y);
                        return true;
                    }
                    else
                    {
                        // one of the positions is relative, read it
                        PlayerController pc = Object.FindObjectOfType<PlayerController>();
                        if (!pc)
                        {
                            sw.WriteLine(
                                IDevCommand.Color("Player not found when requesting relative position.", "red"));
                            return false;
                        }

                        Vector2 offset = Vector2.zero;
                        Vector3 playerPos = pc.transform.position;
                        (bool, bool) usePlayerPos = (false, false);

                        if (args[0].StartsWith('~'))
                        {
                            args[0] = args[0].Remove(0, 1);
                            offset.x = playerPos.x;
                            usePlayerPos.Item1 = true;
                        }

                        if (args[1].StartsWith('~'))
                        {
                            args[1] = args[1].Remove(0, 1);
                            offset.y = playerPos.y;
                            usePlayerPos.Item2 = true;
                        }

                        // check that it either parses or is empty string, i.e. just ~
                        if (!float.TryParse(args[0], out float x) && !(args[0].Length == 0 && usePlayerPos.Item1))
                        {
                            sw.WriteLine(IDevCommand.Color($"Invalid positional argument: {args[0]}", "red"));
                            return false;
                        }

                        if (!float.TryParse(args[1], out float y) && !(args[1].Length == 0 && usePlayerPos.Item2))
                        {
                            sw.WriteLine(IDevCommand.Color($"Invalid positional argument: {args[1]}", "red"));
                            return false;
                        }

                        pos = new Vector2(x, y) + offset;
                        return true;
                    }
                }
                default:
                    return false;
            }
        }
    }
}