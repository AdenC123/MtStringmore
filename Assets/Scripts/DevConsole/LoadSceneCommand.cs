using System.IO;
using UnityEngine.SceneManagement;

namespace DevConsole
{
    /// <summary>
    /// Command to load an arbitrary scene.
    /// </summary>
    public class LoadSceneCommand : IDevCommand
    {
        /// <inheritdoc />s
        public string Name => "scene";
        
        /// <inheritdoc />
        public void Run(string[] args, StringWriter sw)
        {
            if (args.Length != 1)
            {
                sw.WriteLine(IDevCommand.Color($"Usage: {Name} <sceneName>", "red"));
                return;
            }

            // if one of y'all makes a scene called "list.unity" just FYI you will not be able to access it
            // but also, w h y
            if (args[0].ToLower() is "l" or "list")
            {
                for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
                {
                    // this would return blank if the scene isn't loaded, but that's a limitation with Unity
                    // can't do nothin'
                    sw.WriteLine($"Scene {i + 1}: {SceneManager.GetSceneByBuildIndex(i).name}");
                }

                return;
            }
            
            Scene scene = SceneManager.GetSceneByName(args[0]);
            if (!scene.IsValid())
            {
                sw.WriteLine(IDevCommand.Color($"Scene {args[0]} not found.", "red"));
                return;
            }
            SceneManager.LoadScene(scene.buildIndex);
        }
    }
}