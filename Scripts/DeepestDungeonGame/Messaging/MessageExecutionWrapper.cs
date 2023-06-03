using System;

namespace WatStudios.DeepestDungeon.Messaging
{
    /// <summary>
    /// Helps for sorting the messages by adding the ActionExecutionscope as priority
    /// </summary>
    internal struct MessageExecutionWrapper : IComparable<MessageExecutionWrapper>
    {
        internal Action<Message> action;
        internal Message msg;
        internal ActionExecutionScope priority;

        internal MessageExecutionWrapper(Action<Message> action, Message msg, ActionExecutionScope priority)
        {
            this.action = action;
            this.msg = msg;
            this.priority = priority;
        }

        public int CompareTo(MessageExecutionWrapper other)
        {
            return priority.CompareTo(other.priority);
        }
    }
}