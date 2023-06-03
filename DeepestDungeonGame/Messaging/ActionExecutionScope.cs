namespace WatStudios.DeepestDungeon.Messaging
{
    /// <summary>
    /// the lower the number(enum value) the earlier the action gets executed. No effect on the order of messageType
    /// </summary>
    public enum ActionExecutionScope
    {
        Gameplay,
        Default,
        UserInterface
    }
}