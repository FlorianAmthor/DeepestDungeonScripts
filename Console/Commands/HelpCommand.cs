namespace WatStudios.DeepestDungeon.Tools.Console
{
    internal class HelpCommand : ConsoleCommand
    {
        internal HelpCommand()
        {
            Name = "Help";
            Command = "help";
            Description = "Lists all Commands";
            Help = $"<color=red>{Command}</color> for all commands or <color=red>{Command}</color> <color=yellow><CommandName></color> for specific command";
            SuccessReturnText = "If you want help for a single command just type help <commandName>";
            FailureReturnText = "Invalid number of Arguments";
            TakesArguments = true;
        }

        #region Public Methods
        internal override string RunCommand(string[] args)
        {
            if (args.Length == 0)
            {
                foreach (var c in AdminConsole.SortedCommands)
                {
                    AdminConsole.Instance.AddMessageToConsole($"<color=red>{c.Command}</color>: {c.Description}");
                }
                return SuccessReturnText;
            }
            else if (args.Length == 1)
            {
                if (AdminConsole.Commands.TryGetValue(args[0], out ConsoleCommand helpCommand))
                    return $"Help for <color=red>{helpCommand.Name}</color> : {helpCommand.Help}";
                else
                    return $"There is no {args[0]} command";
            }
            else
            {
                return FailureReturnText;
            }
        }
        #endregion

    }
}
