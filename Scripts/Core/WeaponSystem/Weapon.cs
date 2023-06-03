using Photon.Pun;
using System;
using UnityEngine;
using WatStudios.DeepestDungeon.Core.Attributes;
using WatStudios.DeepestDungeon.Messaging;
using NetworkPlayer = WatStudios.DeepestDungeon.Core.PlayerLogic.NetworkPlayer;

namespace WatStudios.DeepestDungeon.Core.WeaponLogic
{
    public class Weapon : MonoBehaviourPun
    {
        #region Private Fields
        private Vector3 _originalPos;
        private Quaternion _originalRot;
        #endregion

        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private WeaponBaseStats _baseStats;
        [SerializeField] private GameObject _armMesh;
        [SerializeField] private Transform _rightHand;
        [SerializeField] private Transform _leftHand;
        [SerializeField] private Transform _aimingHolder;
        [SerializeField] private Vector3 _aimingPos;
        [SerializeField] private Quaternion _aimingRot;
        [SerializeField] private Camera _camera;
#pragma warning restore 649
        #endregion

        #region Properties
        public WeaponBaseStats BaseStats { get => _baseStats;}
        public WeaponCurrentStats CurrentStats { get; private set; }
        internal bool IsShooting { get; set; }
        public Transform RightHand { get => _rightHand; }
        public Transform Lefthand { get => _leftHand; }
        public Vector3 OriginalPos { get => _originalPos; }
        public Quaternion OriginalRot { get => _originalRot; }
        public Transform AimingHolder { get => _aimingHolder; }
        public Vector3 AimingPos { get => _aimingPos; }
        public Quaternion AimingRot { get => _aimingRot; }
        public Camera Camera { get => _camera; }
        #endregion

        #region MonoBehaviour Callbacks
        private void Start()
        {
            if (!photonView.IsMine)
            {
                foreach (var netPlayer in FindObjectsOfType<NetworkPlayer>())
                {
                    if (netPlayer.photonView.Owner == photonView.Owner)
                    {
                        int layer = (int)Math.Log(netPlayer.RemotePlayerMask.value, 2);
                        gameObject.layer = layer;
                        transform.parent = netPlayer.WeaponHolder;
                        foreach (var transform in GetComponentsInChildren<Transform>())
                        {
                            transform.gameObject.layer = layer;
                        }
                        _armMesh.layer = LayerMask.NameToLayer("LocalPlayer");

                        netPlayer.WeaponSystem.RegisterRemotePlayerWeapon(this);
                    }
                }
                Camera.gameObject.SetActive(false);
            }
        }
        #endregion

        #region Internal Methods
        internal void Init()
        {
            CurrentStats = new WeaponCurrentStats(_baseStats);
            _originalPos = transform.localPosition;
            _originalRot = transform.localRotation;
        }

        internal void Reload()
        {
            if (BaseStats.AmmoType == AmmoType.Magazine)
                CurrentStats.Magazine.SetMax();
            else
                //TODO: Cool weapon down in exchange for health?
                return;
        }

        internal void Shoot()
        {
            if (BaseStats.AmmoType == AmmoType.Magazine)
            {
                if (CurrentStats.Magazine.Value > 0)
                {
                    CurrentStats.Magazine.Reduce();
                    MessageHub.SendMessage(MessageType.AmmoChanged, CurrentStats.Magazine.Value);
                }
            }
        }
        #endregion
    }
}