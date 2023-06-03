using ExitGames.Client.Photon;
using Photon.Realtime;
using System;
using UnityEngine;
using WatStudios.DeepestDungeon.Messaging;
using WatStudios.DeepestDungeon.Networking;
using WatStudios.DeepestDungeon.Utility;

namespace WatStudios.DeepestDungeon.Core.Interactables
{
    public class ReadyCheckInteractable : Interactable
    {
        #region Private Fields
        private string _interActionText;
        private NetworkGameEventCode _networkEventCode => NetworkGameEventCode.ReadyCheckInit;
        #endregion

        #region MonoBehaviour Callbacks
        private void Start()
        {
            canInteract = true;
        }

        private void OnEnable()
        {
            MessageHub.Subscribe(MessageType.ReadyCheckInit, OnReadyCheckInit, ActionExecutionScope.Default);
            MessageHub.Subscribe(MessageType.ReadyCheckFail, OnReadyCheckFailed, ActionExecutionScope.Default);
        }

        private void OnDisable()
        {
            MessageHub.Unsubscribe(MessageType.ReadyCheckInit, OnReadyCheckInit);
            MessageHub.Unsubscribe(MessageType.ReadyCheckFail, OnReadyCheckFailed);
        }
        #endregion

        #region Messaging Callbacks
        private void OnReadyCheckInit(Message msg)
        {
            canInteract = false;
        }
        private void OnReadyCheckFailed(Message msg)
        {
            canInteract = true;
        }
        #endregion

        #region Public Methods
        public override void Interact(GameObject interactor)
        {
            NetworkManager.Instance.RaiseNetworkEvent(_networkEventCode, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
        }
        #endregion

        #region Private Methods
        private void SetInteractionValues()
        {
            if (canInteract)
            {
                _interActionText = "Start mission.";
                MessageHub.SendMessage(MessageType.InteractionEnter, _interActionText);
            }
            else
            {
                _interActionText = "Ready check already running.";
                MessageHub.SendMessage(MessageType.InteractionForbidden, _interActionText);
            }
        }
        #endregion

        #region Trigger Callbacks
        private void OnTriggerEnter(Collider other)
        {
            SetInteractionValues();       
        }

        private void OnTriggerStay(Collider other)
        {
            SetInteractionValues();
        }

        private void OnTriggerExit(Collider other)
        {
            try
            {
                MessageHub.SendMessage(MessageType.InteractionExit, _interActionText);
            }
            catch(Exception e)
            {
                Debug.LogError(e);
            }
        }
        #endregion
    }
}
