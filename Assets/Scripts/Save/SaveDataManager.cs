﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Save
{
    /// <summary>
    /// Manager to save and load persistent save data.
    /// </summary>
    public class SaveDataManager : MonoBehaviour
    {
        [SerializeField] private string mainMenuScene;
        
        private Thread _saveThread;
        private string _currentSaveFile;
        private string _activeScene;
        private Vector2? _forcedNextFramePosition;

        private void Awake()
        {
            GameManager.Instance.NewCheckpointReached += OnCheckpointReached;
            SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
        }

        private void OnDestroy()
        {
            SaveFile();
            GameManager.Instance.NewCheckpointReached -= OnCheckpointReached;
        }

        /// <summary>
        /// Called on scene load: sets the position of required objects.
        /// </summary>
        /// <param name="scene">New scene</param>
        /// <param name="mode">Scene load mode</param>
        /// <remarks>
        /// TODO this is hacky
        /// </remarks>
        private void SceneManagerOnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (_forcedNextFramePosition != null)
            {
                // TODO sendMessage is hacky and WILL BREAK
                FindObjectOfType<PlayerController>()?.SendMessage("OnReset");
                FollowCamera followCamera = FindObjectOfType<FollowCamera>();
                if (followCamera)
                {
                    Vector3 position = followCamera.transform.position;
                    position.x = _forcedNextFramePosition.Value.x;
                    position.y = _forcedNextFramePosition.Value.y;
                    followCamera.transform.position = position;
                }
            }

            if (_activeScene != scene.name && scene.name != mainMenuScene)
            {
                // avoid updating the scene name to the title screen LOL
                _activeScene = scene.name;
            }

            _forcedNextFramePosition = null;
        }

        /// <summary>
        /// Called when reaching a checkpoint: saves the file.
        /// </summary>
        /// <param name="checkpoint">New checkpoint position</param>
        private void OnCheckpointReached(Vector2 checkpoint)
        {
            SaveFile();
        }

        /// <summary>
        /// Returns the current save state data.
        /// </summary>
        /// <returns>Current save state date</returns>
        private SaveData GetCurrentSaveData()
        {
            return new SaveData
            {
                checkpointPosition = GameManager.Instance.CheckPointPos,
                checkpointsReached = GameManager.Instance.CheckpointsReached,
                checkpointFacesLeft = GameManager.Instance.RespawnFacingLeft,
                dateTimeBinary = DateTime.Now.ToBinary(),
                sceneName = _activeScene
            };
        }

        /// <summary>
        /// Loads an existing save from a file name.
        /// </summary>
        /// <param name="fileName">File name in the saves folder</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool LoadExistingSave(string fileName)
        {
            string folderLocation = Path.Combine(Application.persistentDataPath, "saves");
            if (!EnsureSaveFolderExists(folderLocation)) return false;
            string filePath = Path.Combine(folderLocation, fileName);
            if (!File.Exists(filePath)) return false;
            try
            {
                SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(filePath));
                SceneManager.LoadScene(data.sceneName);
                GameManager.Instance.UpdateCheckpointData(data.checkpointPosition, data.checkpointFacesLeft, data.checkpointsReached);
                _currentSaveFile = fileName;
                // TODO very hacky
                _forcedNextFramePosition = data.checkpointPosition;

                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Couldn't read from save data: {filePath}: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Creates a new save file.
        /// </summary>
        public void CreateNewSave()
        {
            string folderLocation = Path.Combine(Application.persistentDataPath, "saves");
            if (!EnsureSaveFolderExists(folderLocation)) return;
            int i;
            string outputFile;
            for (i = 0; File.Exists(outputFile = Path.Combine(folderLocation, $"{i}.save")); i++) ;
            try
            {
                File.WriteAllText(outputFile, JsonUtility.ToJson(GetCurrentSaveData()));
                _currentSaveFile = $"{i}.save";
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Couldn't write to save data folder: {outputFile}: {e.Message}");
            }
        }

        public void DeleteSave(string fileName)
        {
            string folderLocation = Path.Combine(Application.persistentDataPath, "saves");
            if (!EnsureSaveFolderExists(folderLocation)) return;
            string filePath = Path.Combine(folderLocation, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            else
            {
                Debug.LogWarning($"Tried to delete file that doesn't exist: {filePath}");
            }
        }

        /// <summary>
        /// Somewhat more thread-safe save file writer.
        /// </summary>
        private class SaveFileWriter
        {
            private readonly string folderPath;
            private readonly string fileName;
            private readonly SaveData saveData;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="folderPath">Folder path of saves directory</param>
            /// <param name="fileName">File name</param>
            /// <param name="saveData">Data to save</param>
            /// <remarks>
            /// <see cref="Application.persistentDataPath"/> needs to be called in the main thread.
            /// </remarks>
            public SaveFileWriter(string folderPath, string fileName, SaveData saveData)
            {
                this.folderPath = folderPath;
                this.fileName = fileName;
                this.saveData = saveData;
            }

            /// <summary>
            /// Saves the data to the save file.
            /// </summary>
            public void SaveFile()
            {
                if (fileName == null)
                {
                    // debug log *should* be threadsafe
                    Debug.LogWarning("Attempted to save with null file path.");
                    return;
                }

                if (!EnsureSaveFolderExists(folderPath)) return;
                string jsonData = JsonUtility.ToJson(saveData);
                string outputFilePath = Path.Combine(folderPath, fileName);
                try
                {
                    File.WriteAllText(Path.Combine(folderPath, fileName), jsonData);
                    Debug.Log($"Wrote to save file: {outputFilePath}");
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Failed to write out save data at {outputFilePath}: {e.Message}");
                }
            }
        }

        /// <summary>
        /// Saves the file: creates the thread to save the file.
        /// </summary>
        /// <remarks>
        /// TODO: do we need to do this on a separate thread? would be easier but would cause freezing at checkpoint reach
        ///
        /// also TODO check this works with web since javascript is singlethreaded
        /// </remarks>
        private void SaveFile()
        {
            if (_currentSaveFile == null) return;
            _saveThread?.Join();
            string folderLocation = Path.Combine(Application.persistentDataPath, "saves");
            SaveFileWriter saveFileWriter = new(folderLocation, _currentSaveFile, GetCurrentSaveData());
            _saveThread = new Thread(saveFileWriter.SaveFile);
            _saveThread.Start();
        }

        /// <summary>
        /// Ensures the provided folder exists by creating it if not found.
        /// </summary>
        /// <param name="folderLocation">Folder path to ensure exists</param>
        /// <returns>True if successful and present, false otherwise</returns>
        private static bool EnsureSaveFolderExists(string folderLocation)
        {
            if (Directory.Exists(folderLocation)) return true;
            try
            {
                Directory.CreateDirectory(folderLocation);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Couldn't create save file directory {folderLocation}: {e.Message}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets all the save files in the save file directory.
        /// </summary>
        /// <returns>Lookup of file name to save data</returns>
        /// <remarks>
        /// If the file couldn't be parsed, the save data will be null.
        /// </remarks>
        public static Dictionary<string, SaveData?> GetSaveFiles()
        {
            string folderLocation = Path.Combine(Application.persistentDataPath, "saves");
            EnsureSaveFolderExists(folderLocation);
            string[] files = Directory.GetFiles(folderLocation);
            Dictionary<string, SaveData?> filesDictionary = new();
            foreach (string file in files)
            {
                try
                {
                    SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(file));
                    filesDictionary.Add(file, data);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Couldn't parse file {file}: {e.Message}");
                    filesDictionary.Add(file, null);
                }
            }

            return filesDictionary;
        }
    }
}
