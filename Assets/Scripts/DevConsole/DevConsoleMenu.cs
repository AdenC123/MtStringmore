﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DevConsole
{
    /// <summary>
    /// Class to handle the developer console and command logic.
    /// </summary>
    public class DevConsoleMenu : MonoBehaviour
    {
        [SerializeField] private KeyCode openKeyCode = KeyCode.F3;
        [SerializeField] private TextMeshProUGUI consoleOutputArea;
        [SerializeField] private ScrollRect scrollRect;

        [SerializeField, Tooltip("Player command input field")]
        private TMP_InputField inputField;

        [SerializeField] private Canvas consoleCanvas;

        [Tooltip("Whether cheats are currently enabled")]
        public bool cheatsEnabled;

        private readonly Dictionary<string, IDevCommand> _commands = new();
        private readonly List<string> _commandHistory = new();
        private int _currentCommandIndex;

        /// <summary>
        /// Utility to get/set the current command index.
        /// </summary>
        private int CurrentCommandIndex
        {
            get => _currentCommandIndex;
            set
            {
                if (_currentCommandIndex == value) return;
                _currentCommandIndex = value;
                inputField.text = _currentCommandIndex < _commandHistory.Count
                    ? _commandHistory[_currentCommandIndex]
                    : "";
                inputField.caretPosition = inputField.text.Length;
            }
        }

        /// <summary>
        /// Simple utility to register a command.
        /// </summary>
        /// <param name="command">Command to register</param>
        private void RegisterCommand(IDevCommand command)
        {
            _commands.Add(command.Name, command);
        }

        /// <summary>
        /// Listener for any Debug.Log* messages.
        /// </summary>
        /// <param name="message">Debug Message</param>
        /// <param name="stacktrace">Stacktrace (that I ignore)</param>
        /// <param name="type">Message type</param>
        private void HandleLog(string message, string stacktrace, LogType type)
        {
            string color = type switch
            {
                LogType.Error or LogType.Exception or LogType.Assert => "red",
                LogType.Warning => "yellow",
                LogType.Log => "white",
                _ => "white"
            };
            consoleOutputArea.text += IDevCommand.Color(message, color) + '\n';
        }

        /// <summary>
        /// Called on input field submit to run commands.
        /// </summary>
        /// <param name="input">Input field contents</param>
        private void OnConsoleSubmit(string input)
        {
            inputField.text = "";
            input = input.Trim();
            if (input.Length == 0) return;

            _commandHistory.Add(input);
            CurrentCommandIndex = _commandHistory.Count;
            consoleOutputArea.text += $"> {input}\n";

            // yes, there's probably a better way of doing this. Too bad!
            string[] inputSplit = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string cmd = inputSplit[0].ToLower();
            string[] args = inputSplit.Skip(1).ToArray();
            if (_commands.TryGetValue(cmd, out IDevCommand command))
            {
                if (!cheatsEnabled && command.RequiresCheats)
                {
                    consoleOutputArea.text +=
                        IDevCommand.Color("Cheats are not enabled in this session.", "red") + '\n';
                }
                else
                {
                    StringWriter stringWriter = new();
                    command.Run(args, stringWriter);
                    consoleOutputArea.text += stringWriter.ToString();
                }
            }
            else
            {
                consoleOutputArea.text += IDevCommand.Color($"Unknown command: {cmd}", "red") + '\n';
            }

            // re-focus input field
            inputField.ActivateInputField();
            // scroll to bottom upon command execution
            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.transform as RectTransform);
            scrollRect.verticalNormalizedPosition = 0;
        }

        private void Awake()
        {
            RegisterCommand(new InvincibilityCommand());
            RegisterCommand(new CheckpointCommand());
            RegisterCommand(new TeleportCommand());
            RegisterCommand(new LoadSceneCommand());
            RegisterCommand(new QuitCommand());
            RegisterCommand(new KillCommand());
            RegisterCommand(new EnableCheatsCommand(this));
            inputField.onSubmit.AddListener(OnConsoleSubmit);
            Application.logMessageReceived += HandleLog;
        }

        private void Update()
        {
            if (Input.GetKeyDown(openKeyCode))
            {
                consoleCanvas.gameObject.SetActive(!consoleCanvas.gameObject.activeSelf);
                if (consoleCanvas.gameObject.activeSelf)
                {
                    inputField.ActivateInputField();
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape) && consoleCanvas.gameObject.activeSelf)
            {
                consoleCanvas.gameObject.SetActive(false);
            }

            // yeah there's definitely a better way to implement the 'up' feature in like every console
            if (inputField.isFocused && _commandHistory.Count > 0)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow) && CurrentCommandIndex > 0)
                {
                    CurrentCommandIndex--;
                }

                if (Input.GetKeyDown(KeyCode.DownArrow) && CurrentCommandIndex < _commandHistory.Count)
                {
                    CurrentCommandIndex++;
                }
            }
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= HandleLog;
        }
    }
}