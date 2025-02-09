using System.IO;

namespace DevConsole
{
    /// <summary>
    /// Interface for a developer command in the Dev Console.
    /// </summary>
    public interface IDevCommand
    {
        /// <summary>
        /// Whether this command requires cheats to be enabled.
        /// </summary>
        bool RequiresCheats => true;
        
        /// <summary>
        /// Command name (i.e. name [args]).
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Executes the command given arguments, writing any log messages to the StringWriter.
        /// </summary>
        /// <param name="args">Command arguments</param>
        /// <param name="sw">StringWriter for log messages</param>
        void Run(string[] args, StringWriter sw);

        /// <summary>
        /// Parses 1 or 0 as true/false.
        /// </summary>
        /// <param name="arg">Argument</param>
        /// <param name="result">Parsed result</param>
        /// <returns>True if parsed successfully, false otherwise</returns>
        protected static bool TryParseBool(string arg, out bool result)
        {
            result = arg == "1";
            return result || arg == "0";
        }

        /// <summary>
        /// Utility function to color rich text.
        /// </summary>
        /// <param name="text">Text to color</param>
        /// <param name="color">Desired color</param>
        /// <returns>Rich text output with correct color set.</returns>
        public static string Color(string text, string color)
        {
            return $"<color={color}>{text}</color>";
        }
    }
}