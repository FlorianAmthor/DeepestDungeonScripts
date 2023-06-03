using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WatStudios.DeepestDungeon.Utility.DataStructures;

namespace WatStudios.DeepestDungeon.Messaging
{
    public class MessageHub : MonoBehaviour
    {
        #region Singleton
        private static MessageHub _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            InitMessageHub();
        }
        #endregion

        #region Private Fields
        /// <summary>
        /// Each Messagetype has a positon in this array. At this position is an array containing all Execution Priorities.
        /// Each Execution Priority has a position which contains a List of actions to this MessageType and ExecutionPriority. 
        /// They will all be executed if the MessageType gets triggered once. The order of execution depends on the priority of the action.
        /// </summary>
        private HashSet<Action<Message>>[][] _messageActionArray;
        /// <summary>
        /// The queue containg all messages that are not yet handled. 
        /// </summary>
        private Queue<Message> _messageQueue;
        /// <summary>
        /// The queue containing all messages that should be subscribed or unsubscribed in this frame
        /// </summary>
        private Queue<Action> _subscriptionsQueue;
        /// <summary>
        /// The queue containing all messages that should be executed in this frame
        /// </summary>
        private PriorityQueue<MessageExecutionWrapper> _executionQueue;
        /// <summary>
        /// Number of types in enum MessageTypes
        /// </summary>
        private int _numberOfMessageTypes;
        /// <summary>
        /// Number of types in enum ActionExecutionPriorities
        /// </summary>
        private int _numberOfPriorities;
        #endregion

        #region MonoBehaviour Callbacks
        //Sending the messages happens here
        private void LateUpdate()
        {
            while (_subscriptionsQueue.Count > 0)
            {
                _subscriptionsQueue.Dequeue().Invoke();
            }
            HandleMessageQueue();
            SendAllMessages();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Initializes the Message Hub
        /// </summary>
        private void InitMessageHub()
        {
            _messageQueue = new Queue<Message>();
            _subscriptionsQueue = new Queue<Action>();
            _executionQueue = new PriorityQueue<MessageExecutionWrapper>(true);
            _numberOfMessageTypes = Enum.GetValues(typeof(MessageType)).Cast<int>().Max() + 1;
            _numberOfPriorities = Enum.GetValues(typeof(ActionExecutionScope)).Cast<int>().Max() + 1;
            //Sets the length of the array so that all enums fit in it
            _messageActionArray = new HashSet<Action<Message>>[_numberOfMessageTypes][];

            //init data structure
            for (int msgTypeIndex = 0; msgTypeIndex < _messageActionArray.Length; msgTypeIndex++)
            {
                _messageActionArray[msgTypeIndex] = new HashSet<Action<Message>>[_numberOfPriorities];
                for (int prioIndex = 0; prioIndex < _numberOfPriorities; prioIndex++)
                {
                    _messageActionArray[msgTypeIndex][prioIndex] = new HashSet<Action<Message>>();
                }
            }
        }

        /// <summary>
        /// All messages in _messageQueue will be handle and prepared for execution
        /// </summary>
        private void HandleMessageQueue()
        {
            Message[] currentMessageArray = new Message[_messageQueue.Count];
            _messageQueue.CopyTo(currentMessageArray, 0);
            _messageQueue.Clear();
            Queue<Message> currentMessageQueue = new Queue<Message>(currentMessageArray);

            while (currentMessageQueue.Count > 0)
            {
                Message msg = currentMessageQueue.Dequeue();
                bool someoneListening = false;
                for (int prioIndex = 0; prioIndex < _numberOfPriorities; prioIndex++)
                {
                    if (_messageActionArray[(int)msg.Type][prioIndex].Count == 0)
                        continue;
                    someoneListening = true;
                    foreach (var action in _messageActionArray[(int)msg.Type][prioIndex])
                    {
                        _executionQueue.Enqueue(new MessageExecutionWrapper(action, msg, (ActionExecutionScope)prioIndex));
                    }
                }
                if (!someoneListening)
                {
                    Debug.LogWarning("(Custom) Nothing is Listening to the MessageType: " + msg.Type);
                }
            }
        }

        /// <summary>
        /// Sends all messages for this frame
        /// </summary>
        private void SendAllMessages()
        {
            while (_executionQueue.Count != 0)
            {
                var msgExecWrapper = _executionQueue.Dequeue();
                msgExecWrapper.action.Invoke(msgExecWrapper.msg);
            }
        }

        /// <summary>
        /// Unsubscribes <paramref name="action"/> from the _messageActionArray at index <paramref name="msgType"/>
        /// </summary>
        /// <param name="msgType">Type of the Message</param>
        /// <param name="action">Action of to subscribe</param>
        /// <param name="actionPrio">Priority of the Message</param>
        private static void SubscribeMessage(MessageType msgType, Action<Message> action, ActionExecutionScope actionPrio)
        {
            for (int prioIndex = 0; prioIndex < _instance._messageActionArray[(int)msgType].Length; prioIndex++)
            {
                if (!_instance._messageActionArray[(int)msgType][prioIndex].Contains(action))
                    continue;
                Debug.LogError("(Custom) There is already a Action " + action.Method.ToString() + "subscribed to the MessageType " + msgType + ". You can not Subscribe twice with the same action to the same MessageType.");
                return;
            }
            _instance._messageActionArray[(int)msgType][(int)actionPrio].Add(action);
        }

        /// <summary>
        /// Unsubscribes <paramref name="action"/> from the _messageActionArray at index <paramref name="msgType"/>
        /// </summary>
        /// <param name="msgType">Type of the Message</param>
        /// <param name="action">Action of to unsubscribe</param>
        private static void UnsubscribeMessage(MessageType msgType, Action<Message> action)
        {
            for (int prioIndex = 0; prioIndex < _instance._messageActionArray[(int)msgType].Length; prioIndex++)
            {
                if (_instance._messageActionArray[(int)msgType][prioIndex].Contains(action))
                {
                    _instance._messageActionArray[(int)msgType][prioIndex].Remove(action);
                    return;
                }
            }
            Debug.LogWarning("(Custom) You are trying to remove a Action that wasn't subscribed before");
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds a Action to a certain messageType. The same action can't be added twice to the same MessageType!
        /// </summary>
        /// <param name="msgType">Any predefined MessageType</param>
        /// <param name="action">Function to be invoked when the message gets called</param>
        /// <param name="actionPrio">Dictates the execution order of the Actions associated to that MessageType</param>
        public static void Subscribe(MessageType msgType, Action<Message> action, ActionExecutionScope actionPrio)
        {
            _instance._subscriptionsQueue.Enqueue(new Action(() => SubscribeMessage(msgType, action, actionPrio)));
        }
        /// <summary>
        /// Unsubscribes an action from the given MessageType.
        /// </summary>
        /// <param name="msgType"></param>
        /// <param name="action"></param>
        public static void Unsubscribe(MessageType msgType, Action<Message> action)
        {
            _instance._subscriptionsQueue.Enqueue(new Action(() => UnsubscribeMessage(msgType, action)));
        }
        /// <summary>
        /// Sends a message of <paramref name="messageType"/> with the given <paramref name="data"/>
        /// </summary>
        /// <param name="messageType"> Type of the message</param>
        /// <param name="data">Data of the message</param>
        public static void SendMessage(MessageType messageType, params object[] data)
        {
            Message msg = new Message(messageType, data);
            _instance._messageQueue.Enqueue(msg);
        }
        #endregion
    }
}