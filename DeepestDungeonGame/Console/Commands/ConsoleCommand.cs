namespace WatStudios.DeepestDungeon.Tools.Console
{
    internal abstract class ConsoleCommand
    {
        #region Public Fields
        /// <summary>
        /// Name of the command
        /// </summary>
        internal string Name { get; set; }
        /// <summary>
        /// command to be registered with the console
        /// </summary>
        internal string Command { get; set; }
        /// <summary>
        /// Command description
        /// </summary>
        internal string Description { get; set; }
        /// <summary>
        /// How to use the command
        /// </summary>
        internal string Help { get; set; }
        /// <summary>
        /// The string returned by the Command when executed successfully
        /// </summary>
        internal string SuccessReturnText { get; set; }
        /// <summary>
        /// The string returned by the Command when executed with failure
        /// </summary>
        internal string FailureReturnText { get; set; }
        /// <summary>
        /// Depicts if the command can be customized with arguments
        /// </summary>
        internal bool TakesArguments { get; set; }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Forces the Command string to lowercase
        /// </summary>
        internal void ForceCommandToLower()
        {
            Command = Command.ToLower();
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Executes the command with extra arguments in <paramref name="args"/>
        /// </summary>
        /// <param name="args"></param>
        /// <returns>A string if the command was executed successfully or in failue. Empty when success or failure can't be determined.</returns>
        internal abstract string RunCommand(string[] args);
        #endregion

    }
}
