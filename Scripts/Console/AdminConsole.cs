using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using WatStudios.DeepestDungeon.Messaging;

namespace WatStudios.DeepestDungeon.Tools.Console
{
    public class AdminConsole : MonoBehaviour
    {
        #region Singleton
        public static AdminConsole Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Commands = new Dictionary<string, ConsoleCommand>();
            }
        }

        #endregion

        #region Private Fields
        private List<string> _history;
        private int _historyIndex;
        #endregion

        #region Exposed Private Fields
#pragma warning disable 649
        [Header("UI Components")]
        [SerializeField]
        private Canvas _consoleCanvas;
        [SerializeField]
        private ScrollRect _scrollRect;
        [SerializeField]
        private Text _consoleText;
        [SerializeField]
        private Text _inputText;
        [SerializeField]
        private InputField _inputField;
#pragma warning restore 649
        #endregion

        #region Public Fields
        /// <summary>
        /// Command Registry
        /// </summary>
        internal static Dictionary<string, ConsoleCommand> Commands { get; private set; }
        internal static List<ConsoleCommand> SortedCommands { get; private set; }
        #endregion

        #region MonoBehaviour Callbacks

        private void Start()
        {
            _history = new List<string>();
            _history.Add(string.Empty);
            _consoleCanvas.gameObject.SetActive(false);
            _historyIndex = 0;
            CreateCommands();
            SortCommands();
        }

        private void SortCommands()
        {
            SortedCommands = Commands.Values.ToList();
            SortedCommands.Sort((cmd1, cmd2) => cmd1.Name.CompareTo(cmd2.Name));
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.F12))
            {
                SwitchConsoleState();
            }

            if (_consoleCanvas.gameObject.activeInHierarchy)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    if (_inputText.text != "")
                    {
                        AddMessageToConsole(_inputText.text);
                        HandleInput(_inputText.text);
                        _inputField.ActivateInputField();
                        _inputField.text = "";
                        _historyIndex = 0;
                    }
                }
                if (Input.GetKeyDown(KeyCode.Backspace))
                    _historyIndex = 0;

                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    _historyIndex++;
                    if (_historyIndex >= _history.Count)
                        _historyIndex = _history.Count - 1;
                    if(_history.Count != 0)
                        _inputField.text = _history[_historyIndex];
                    _inputField.MoveTextEnd(false);
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    _historyIndex--;
                    if (_historyIndex < 0)
                        _historyIndex = 0;
                    if (_history.Count != 0)
                        _inputField.text = _history[_historyIndex];
                    _inputField.MoveTextEnd(false);
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Switches the active state of the console window
        /// </summary>
        private void SwitchConsoleState()
        {
            _consoleCanvas.gameObject.SetActive(!_consoleCanvas.gameObject.activeInHierarchy);
            MessageHub.SendMessage(MessageType.AllowPlayerInput,!_consoleCanvas.gameObject.activeInHierarchy);
            if (_consoleCanvas.gameObject.activeInHierarchy)
                _inputField.ActivateInputField();
        }

        /// <summary>
        /// Registers all commands that inherit from ConsoleCommand
        /// </summary>
        private void CreateCommands()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(ConsoleCommand));
            List<Type> commandTypes = assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(ConsoleCommand))).ToList();

            foreach (Type commandType in commandTypes)
            {
                ConsoleCommand command = Activator.CreateInstance(commandType, true) as ConsoleCommand;
                command.ForceCommandToLower();
                AddCommandToConsole(command);
            }
        }

        /// <summary>
        /// Handles the given <paramref name="input"/>
        /// </summary>
        /// <param name="input">String to be handled</param>
        private void HandleInput(string input)
        {
            string[] splitInput = input.Split(' ');
            splitInput = splitInput.Where(s => !string.IsNullOrEmpty(s)).ToArray();

            if (input.Length == 0 || input == null)
            {
                AddMessageToConsole("No such command. Type /help for a list of commands.");
                return;
            }

            if (!Commands.ContainsKey(splitInput[0]))
            {
                AddMessageToConsole("No such command.Type / help for a list of commands.");
            }
            else
            {
                ConsoleCommand command = Commands[splitInput[0]];
                AddCommandToHistory(splitInput);
                if (command.Name == "Help")
                {
                    AddMessageToConsole(command.RunCommand(splitInput.Skip(1).ToArray()));
                    return;
                }

                if (command.TakesArguments)
                {
                    if (splitInput.Length <= 1)
                    {
                        AddMessageToConsole($"{command.Name} needs arguments!");
                    }
                    else
                    {
                        AddMessageToConsole(command.RunCommand(splitInput.Skip(1).ToArray()));
                    }
                }
                else
                {
                    if (splitInput.Length > 1)
                    {
                        AddMessageToConsole($"{command.Name} doesn't need any arguments!");
                    }
                    else
                    {
                        AddMessageToConsole(command.RunCommand(null));
                    }
                }
            }
        }

        /// <summary>
        /// Updates the Command Registry with the command string as key and <paramref name="command"/> as value
        /// </summary>
        /// <param name="command"></param>
        private void AddCommandToConsole(ConsoleCommand command)
        {
            if (!Commands.ContainsKey(command.Command))
            {
                Commands.Add(command.Command, command);
            }
        }

        /// <summary>
        /// Adds the string in <paramref name="msg"/> to the console text field
        /// </summary>
        /// <param name="msg"></param>
        internal void AddMessageToConsole(string msg)
        {
            if (msg == string.Empty)
                return;
            _consoleText.text += msg + "\n";
            _scrollRect.verticalNormalizedPosition = 0f;
        }

        internal void AddCommandToHistory(string[] arguments)
        {
            string historyString = string.Empty;
            foreach (var s in arguments)
            {
                historyString += $"{s} ";
            }
            _history.Insert(0, historyString);
        }

        #endregion
    }
}
