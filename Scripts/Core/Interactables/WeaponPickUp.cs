using Photon.Pun;
using UnityEngine;
using WatStudios.DeepestDungeon.Messaging;
using WatStudios.DeepestDungeon.Utility;
using WatStudios.DeepestDungeon.Core.WeaponLogic;
using NetworkPlayer = WatStudios.DeepestDungeon.Core.PlayerLogic.NetworkPlayer;

namespace WatStudios.DeepestDungeon.Core.Interactables
{
    public class WeaponPickUp : Interactable
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private GameObject _weaponPrefab;
#pragma warning restore 649
        #endregion

        #region Private Fields
        private string _interActionText;
        private ActiveWeaponSlot _activeSlot;
        #endregion

        #region Public Methods
        public override void Interact(GameObject interactor)
        {
            if (CanInteract)
            {
                var netPlayer = interactor.GetComponent<NetworkPlayer>();
                netPlayer.WeaponSystem.photonView.RPC("SwitchWeapon", RpcTarget.All, (int)netPlayer.WeaponSystem.ActiveWeaponSlot, _weaponPrefab.name);
            }
        }
        #endregion

        #region Private Methods
        private void SetInteractionValues(ActiveWeaponSlot activeSlot)
        {
            if (activeSlot != ActiveWeaponSlot.Unarmed)
            {
                canInteract = true;
                _interActionText = $"Pick up {_weaponPrefab.GetComponent<Weapon>().BaseStats.Name}";
                MessageHub.SendMessage(MessageType.InteractionEnter, _interActionText);
            }
            else
            {
                canInteract = false;
                _interActionText = "You need to switch with one of your weapons.";
                MessageHub.SendMessage(MessageType.InteractionForbidden, _interActionText);
            }
        }
        #endregion

        #region Trigger Callbacks
        private void OnTriggerEnter(Collider other)
        {
            _activeSlot = other.GetComponent<NetworkPlayer>().WeaponSystem.ActiveWeaponSlot;
            SetInteractionValues(_activeSlot);
        }

        private void OnTriggerStay(Collider other)
        {
            var slot = other.GetComponent<NetworkPlayer>().WeaponSystem.ActiveWeaponSlot;
            if (slot != _activeSlot)
            {
                _activeSlot = slot;
                SetInteractionValues(_activeSlot);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            MessageHub.SendMessage(MessageType.InteractionExit, _interActionText);
        }
        #endregion
    }
}