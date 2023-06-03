namespace WatStudios.DeepestDungeon.Messaging
{
    /// <summary>
    /// Class for messages with type and data.
    /// </summary>
    public class Message
    {
        public readonly MessageType Type;
        public readonly object[] Data;

        /// <summary>
        /// Constructor for messages with data
        /// </summary>
        /// <param name="type">messageType</param>
        /// <param name="data">what ever you want to transport with the Message</param>
        internal Message(MessageType type, object[] data)
        {
            Type = type;
            Data = data;
        }
    }
}