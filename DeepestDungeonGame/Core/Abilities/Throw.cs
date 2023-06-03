using Photon.Pun;
using UnityEngine;
using WatStudios.DeepestDungeon.Core.WeaponLogic;
using WatStudios.DeepestDungeon.Messaging;
using WatStudios.DeepestDungeon.Utility;
using NetworkPlayer = WatStudios.DeepestDungeon.Core.PlayerLogic.NetworkPlayer;

namespace WatStudios.DeepestDungeon.Core.AbilitySystem.Abilities
{
    [CreateAssetMenu(fileName = "Throw", menuName = "ScriptableObjects/Abilities/Throw")]
    public class Throw : Ability
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private GameObject _throwableObject;
        [SerializeField] private float _throwForce;
        [SerializeField] private int _previewDetail;
#pragma warning restore 649
        #endregion

        #region Private Fields
        private Grenade _instancedObject;
        private Vector3 _vel;
        #endregion

        #region ScriptableObject Methods
        private void OnEnable()
        {
            IsUsable = true;
        }
        #endregion

        #region Public Methods
        public override void Tick(NetworkPlayer nPlayer)
        {
            if (cooldown.Value > 0)
            {
                if (IsUsable)
                    IsUsable = false;
                OnCooldown();
            }
            if (cooldown.Value <= 0 && !IsUsable)
            {
                IsUsable = true;
                MessageHub.SendMessage(MessageType.AbilityCooldownEnd, name);
            }
        }

        public override void Use(NetworkPlayer nPlayer, bool castOnSelf)
        {
            _instancedObject.GetComponent<LineRenderer>().enabled = false;
            _instancedObject.GetComponent<Grenade>().StartTicking();
            _instancedObject.GetComponent<Rigidbody>().useGravity = true;
            _instancedObject.transform.forward = nPlayer.FpsCamera.transform.forward;
            _instancedObject.GetComponent<Rigidbody>().velocity = _vel;
            _instancedObject = null;

            cooldown.SetBase();
            MessageHub.SendMessage(MessageType.AbilityCooldownStart, name);
        }

        public override void Preview(bool previewActive, bool abilityCanceled, NetworkPlayer nPlayer = null)
        {
            if (previewActive)
            {
                if (NetworkPlayer.LocalPlayerInstance.WeaponSystem.ActiveWeaponSlot != ActiveWeaponSlot.Unarmed)
                    NetworkPlayer.LocalPlayerInstance.WeaponSystem.HandleWeapon(ActiveWeaponSlot.Unarmed);
                Vector3 grenadePos = nPlayer.FpsCamera.transform.position + nPlayer.FpsCamera.transform.right / 2 + nPlayer.FpsCamera.transform.forward / 2;

                if (_instancedObject == null)
                {
                    _instancedObject = PhotonNetwork.Instantiate(_throwableObject.name, grenadePos, Quaternion.identity).GetComponent<Grenade>();
                    _instancedObject.AddBonusDamage(nPlayer.CurrentStats.DamageModifier.FlatValue, nPlayer.CurrentStats.DamageModifier.PercentualValue);
                }
                else
                {
                    _instancedObject.transform.position = grenadePos;
                }
                _instancedObject.transform.forward = nPlayer.FpsCamera.transform.forward;
                _instancedObject.GetComponent<Rigidbody>().useGravity = false;
                _vel = TrajectoryCalculator.RenderArc(_throwForce, TrajectoryCalculator.CalculateAngle(nPlayer.FpsCamera.transform), _previewDetail, nPlayer.FpsCamera.transform, _instancedObject.transform, _instancedObject.GetComponent<LineRenderer>(), _instancedObject.BounceLayer);
            }
            else
            {
                if (abilityCanceled)
                {
                    _instancedObject.GetComponent<LineRenderer>().enabled = false;
                    PhotonNetwork.Destroy(_instancedObject.gameObject);
                    _instancedObject = null;
                }
                NetworkPlayer.LocalPlayerInstance.WeaponSystem.HandleWeapon(NetworkPlayer.LocalPlayerInstance.WeaponSystem.PreviousActiveWeaponSlot);
            }
        }
        #endregion
    }
}