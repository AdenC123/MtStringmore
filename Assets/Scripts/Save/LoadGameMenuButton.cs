using System;
using System.IO;
using TMPro;
using UnityEngine;

namespace Save
{
    /// <summary>
    /// Load game menu button logic.
    /// </summary>
    [DisallowMultipleComponent]
    public class LoadGameMenuButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI sceneNameText;
        [SerializeField] private TextMeshProUGUI fileNameText;
        [SerializeField] private TextMeshProUGUI checkpointNumText;
        [SerializeField] private TextMeshProUGUI dateTimeText;

        private LoadGameMenu _loadGameMenu;
        private string _fileName;

        private void Awake()
        {
            _loadGameMenu = GetComponentInParent<LoadGameMenu>();
        }

        /// <summary>
        /// Initializes all the text fields to the correct values.
        /// </summary>
        /// <param name="saveData">Save data</param>
        /// <param name="fileName">File name of save</param>
        public void Initialize(SaveData saveData, string fileName)
        {
            sceneNameText.text = saveData.sceneName;
            fileNameText.text = Path.GetFileNameWithoutExtension(fileName);
            checkpointNumText.text = $"Checkpoint {saveData.checkpointsReached.Length}";
            dateTimeText.text = DateTime.FromBinary(saveData.dateTimeBinary).ToString("g");
            _fileName = fileName;
        }

        /// <summary>
        /// Called by the button: tells the load game menu that this is now active
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public void OnButtonClick()
        {
            _loadGameMenu.SetActiveSave(_fileName);
        }
    }
}
