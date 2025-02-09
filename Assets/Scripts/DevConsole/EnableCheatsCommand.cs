using System.IO;

namespace DevConsole
{
    /// <summary>
    /// Command to enable cheats. Yes, that command looks very familiar.
    /// </summary>
    public class EnableCheatsCommand : IDevCommand
    {
        public bool RequiresCheats => false;
        
        public string Name => "sv_cheats";

        private readonly DevConsoleMenu _devConsole;

        public EnableCheatsCommand(DevConsoleMenu menu)
        {
            _devConsole = menu;
        }
        
        public void Run(string[] args, StringWriter sw)
        {
            if (args.Length != 1 || !IDevCommand.TryParseBool(args[0], out bool arg))
            {
                sw.WriteLine(IDevCommand.Color($"Usage: {Name} <1/0>", "red"));
                return;
            }
            
            _devConsole.cheatsEnabled = arg;
        }
    }
}