using UnityEngine;
using WatStudios.DeepestDungeon.Core.Utiliy.Areas;
using WatStudios.DeepestDungeon.Messaging;
using NetworkPlayer = WatStudios.DeepestDungeon.Core.PlayerLogic.NetworkPlayer;

namespace WatStudios.DeepestDungeon.Core.AbilitySystem.Abilities
{
    [CreateAssetMenu(fileName = "HealAura", menuName = "ScriptableObjects/Abilities/HealAura")]
    public class HealAura : Ability
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private float _secondBetweenTicks;
        [SerializeField] private SphereArea _circleArea;
        [SerializeField] private int _healPerTick;
#pragma warning restore 649
        #endregion

        #region Private Fields
        private bool _isActive;
        private float _lastTimeTicked;
        private float _lastTimeToggled;
        #endregion

        #region ScriptableObject Methods
        private void OnEnable()
        {
            IsUsable = true;
            _isActive = false;
            _lastTimeToggled = 0;
            _lastTimeTicked = 0;
        }
        #endregion

        #region Public Methods
        public override void Tick(NetworkPlayer nPlayer)
        {
            float time = Time.time;

            if (!IsUsable)
            {
                if (_lastTimeToggled + cooldown.BaseValue <= time)
                {
                    IsUsable = true;
                    MessageHub.SendMessage(MessageType.AbilityCooldownEnd, name);
                }
                else
                    OnCooldown();
            }

            if (_isActive && _lastTimeTicked + _secondBetweenTicks <= time)
            {
                _lastTimeTicked = time;
                foreach (var collider in _circleArea.GetCollidingObjects(nPlayer.transform.position, nPlayer.transform.forward, afflictedObjects))
                {
                    var colPlayer = collider.GetComponent<NetworkPlayer>();
                    if (!DatabaseManager.TryGetId(statusEffect, out int statusEffectId))
                        Debug.LogError("No such element in the database!");
                    colPlayer.StatusEffectHandler.photonView.RPC("AddStatusEffect", colPlayer.PhotonPlayer, statusEffectId, nPlayer.photonView.ViewID);
                    colPlayer.photonView.RPC("RecieveHealing", colPlayer.PhotonPlayer, _healPerTick);
                }
            }
        }

        public override void Use(NetworkPlayer nPlayer, bool castOnSelf)
        {
            float time = Time.time;
            if (_lastTimeToggled + cooldown.BaseValue <= time)
            {
                _isActive = !_isActive;
                IsUsable = false;
                _lastTimeToggled = time;
                cooldown.SetBase();
                MessageHub.SendMessage(MessageType.AbilityCooldownStart, name);
            }
        }
        #endregion
    }
}