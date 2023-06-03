using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using WatStudios.DeepestDungeon.Core.EnemyLogic;
using WatStudios.DeepestDungeon.Messaging;
using NetworkPlayer = WatStudios.DeepestDungeon.Core.PlayerLogic.NetworkPlayer;

namespace WatStudios.DeepestDungeon.Core.AbilitySystem.StatusEffects
{
    public class StatusEffectHandler : MonoBehaviourPun
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private GameObject _ownerObj;
#pragma warning restore 649
        #endregion

        #region Private Fields
        private IStatusEntity _owner;
        private bool _playerOwner;
        private Dictionary<Type, StatusEffect> _statusEffects;
        private List<StatusEffect> _expiredStatusEffects;
        #endregion

        #region MonoBehaviour Callbacks
        private void OnEnable()
        {
            _statusEffects = new Dictionary<Type, StatusEffect>();
            _expiredStatusEffects = new List<StatusEffect>();
            _playerOwner = true;
            _owner = _ownerObj.GetComponent<NetworkPlayer>();
            if (_owner == null)
            {
                _owner = _ownerObj.GetComponent<EnemyEntity>();
                _playerOwner = false;
            }
        }

        private void Update()
        {
            if (!photonView.IsMine || !_playerOwner)
                return;
            Tick();
        }

        private void LateUpdate()
        {
            if (!photonView.IsMine || !_playerOwner)
                return;
            LateTick();
        }

        internal void Tick()
        {
            if (_statusEffects.Count == 0 || !photonView.IsMine)
                return;
            foreach (var pair in _statusEffects)
            {
                if (pair.Value.IsExpired(_owner))
                {
                    _expiredStatusEffects.Add(pair.Value);
                }
                else
                {
                    pair.Value.Tick(_owner);
                    pair.Value.Duration.Reduce();
                    if (_playerOwner)
                        MessageHub.SendMessage(MessageType.BuffTimeLeft, pair.Key, pair.Value.Duration.Value);
                }
            }
        }

        internal void LateTick()
        {
            if (!photonView.IsMine)
                return;
            if (_expiredStatusEffects.Count != 0)
            {
                var statusEffect = _expiredStatusEffects[0];
                statusEffect.Undo(_owner);
                if (_playerOwner)
                    MessageHub.SendMessage(MessageType.BuffEnd, statusEffect.GetType());
                _statusEffects.Remove(statusEffect.GetType());
                _expiredStatusEffects.RemoveAt(0);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Checks if the entity is afflicted by the given <paramref name="statusEffect"/>
        /// </summary>
        /// <param name="statusEffect">The statuseffect to check against</param>
        /// <returns></returns>
        public bool IsAfflictedBy(StatusEffect statusEffect)
        {
            return _statusEffects.ContainsKey(statusEffect.GetType());
        }

        /// <summary>
        /// Checks if the entity is afflicted by the given <paramref name="statusEffectType"/>
        /// </summary>
        /// <param name="statusEffectType">The type statuseffect to check against</param>
        /// <returns></returns>
        public bool IsAfflictedBy(Type statusEffectType)
        {
            return _statusEffects.ContainsKey(statusEffectType);
        }

        /// <summary>
        /// Tries to get a status effect by type <paramref name="statusEffectType"/>
        /// </summary>
        /// <param name="statusEffectType">The type of the statuseffect to get</param>
        /// <returns></returns>
        public bool TryGetStatusEffect<T>(Type statusEffectType, out T statusEffect) where T : StatusEffect
        {
            if (_statusEffects.TryGetValue(statusEffectType, out StatusEffect effect))
            {
                statusEffect = effect as T;
                return true;
            }
            statusEffect = null;
            return false;
        }

        /// <summary>
        /// Checks if the entity is afflicted by the given <paramref name="statusEffectId"/>
        /// </summary>
        /// <param name="statusEffectId"></param>
        /// <returns></returns>
        public bool IsAfflictedBy(int statusEffectId)
        {
            if (DatabaseManager.TryGetElement(statusEffectId, out StatusEffect statusEffect))
                return _statusEffects.ContainsKey(statusEffect.GetType());
            else
                return false;
        }

        /// <summary>
        /// Adds the given <paramref name="statusEffect"/> or refreshes it if it already exists
        /// </summary>
        /// <param name="statusEffect"></param>
        [PunRPC]
        public void AddStatusEffect(int statusEffectId, int sourcePhotonViewId)
        {
            if (!DatabaseManager.TryGetElement(statusEffectId, out StatusEffect statusEffect))
            {
                Debug.LogError($"No element in data base for the statusEffectId: {statusEffectId}");
                return;
            }

            if (_statusEffects.TryGetValue(statusEffect.GetType(), out StatusEffect localEffect))
            {
                localEffect.Refresh(_owner);
                var photonView = PhotonNetwork.GetPhotonView(sourcePhotonViewId);
                localEffect.Source = photonView.GetComponent<NetworkPlayer>() != null ? photonView.GetComponent<NetworkPlayer>() as IStatusEntity : photonView.GetComponent<EnemyEntity>() as IStatusEntity;
                localEffect.Duration.SetBase();
                if (_playerOwner)
                {
                    if (localEffect.IsStackable)
                        MessageHub.SendMessage(MessageType.BuffRefresh, localEffect.GetType(), localEffect.NumOfStacks(_owner), localEffect.Duration.BaseValue);
                    else
                        MessageHub.SendMessage(MessageType.BuffRefresh, localEffect.GetType(), -1, localEffect.Duration.BaseValue);
                }
            }
            else
            {
                localEffect = Instantiate(statusEffect);
                var photonView = PhotonNetwork.GetPhotonView(sourcePhotonViewId);
                localEffect.Source = photonView.GetComponent<NetworkPlayer>() != null ? photonView.GetComponent<NetworkPlayer>() as IStatusEntity : photonView.GetComponent<EnemyEntity>() as IStatusEntity;
                localEffect.Duration.SetBase();
                localEffect.Apply(_owner);
                _statusEffects.Add(localEffect.GetType(), localEffect);
                if (_playerOwner)
                    MessageHub.SendMessage(MessageType.BuffStart, localEffect.GetType(), localEffect.BuffSprite, localEffect.IsBuff, localEffect.IsStackable, localEffect.Duration.BaseValue);
            }
            if (_playerOwner)
                MessageHub.SendMessage(MessageType.BuffTimeLeft, localEffect.GetType(), localEffect.Duration.Value);
        }

        /// <summary>
        /// Removes and undoes the given <paramref name="statusEffect"/>
        /// </summary>
        /// <param name="statusEffect"></param>
        /// <returns>If the statuseffect was removed</returns>
        [PunRPC]
        public bool RemoveStatusEffect(StatusEffect statusEffect)
        {
            if (_statusEffects.TryGetValue(statusEffect.GetType(), out StatusEffect localEffect))
            {
                localEffect.Undo(_owner);
                _statusEffects.Remove(localEffect.GetType());
                if (_playerOwner)
                    MessageHub.SendMessage(MessageType.BuffEnd, localEffect.GetType());
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes and undoes all statuseffects
        /// </summary>
        [PunRPC]
        public void ResetAllStatusEffects()
        {
            foreach (var pair in _statusEffects)
            {
                pair.Value.Undo(_owner);
                if (_playerOwner)
                    MessageHub.SendMessage(MessageType.BuffEnd, pair.Key);
            }
            _statusEffects.Clear();
        }
        #endregion
    }
}