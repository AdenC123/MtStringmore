﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Interactables;
using Player;
using UnityEngine;

namespace DevConsole
{
    /// <summary>
    /// "Quality of life" modifications.
    ///
    /// cheaters get banned
    /// </summary>
    public class QualityOfLifeCommand : IDevCommand
    {
        /// <inheritdoc />
        public string Name => "QualityOfLife";

        /// <inheritdoc />
        public string[] Aliases => new[]{"qol"};

        /// <inheritdoc />
        public void Run(string[] args, StringWriter sw)
        {
            if (args.Length > 0)
            {
                PrintUsage(sw);
                return;
            }
            PlayerController player = Object.FindObjectOfType<PlayerController>();
            if (!player)
            {
                sw.WriteLine(IDevCommand.Color("Couldn't find player in current scene.", "red"));
                return;
            }
            
            Checkpoint checkpoint = Object.FindObjectsOfType<Checkpoint>()
                .FirstOrDefault(checkpoint => checkpoint.HasConversation);
            if (!checkpoint)
            {
                sw.WriteLine(IDevCommand.Color("Couldn't find final checkpoint in current scene.", "red"));
                return;
            }
            
            Camera camera = Camera.main;
            if (!camera)
            {
                sw.WriteLine(IDevCommand.Color("Couldn't find camera in current scene.", "yellow"));
            }

            player.StartCoroutine(Hack(checkpoint.transform.position, player, camera));
        }

        /// <inheritdoc />
        public void PrintUsage(StringWriter sw, string color = "red")
        {
            sw.WriteLine(IDevCommand.Color($"Usage: {Name}", color));
        }

        /// <summary>
        /// Forcibly sets player location to collectible/end checkpoint location every frame.
        /// </summary>
        /// <param name="position">End checkpoint location</param>
        /// <param name="player">Player</param>
        /// <param name="camera"></param>
        /// <returns>Coroutine</returns>
        private static IEnumerator Hack(Vector3 position, PlayerController player, Camera camera)
        {
            List<Collectable> collectables = new(Object.FindObjectsOfType<Collectable>());
            while (collectables.Count > 0)
            {
                Collectable collectable = collectables.OrderBy(collectable =>
                    Vector2.Distance(collectable.transform.position, player.transform.position)).First();
                player.transform.position = collectable.transform.position;
                if (camera)
                    camera.transform.position = collectable.transform.position;
                collectables.Remove(collectable);
                yield return null;
            }
            player.transform.position = position;
            if (camera) 
                camera.transform.position = position;
        }
    }
}
