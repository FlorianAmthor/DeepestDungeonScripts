using UnityEngine;
using WatStudios.DeepestDungeon.Core.AbilitySystem.StatusEffects;
using WatStudios.DeepestDungeon.Core.EnemyLogic.ThreatSystem;
using WatStudios.DeepestDungeon.Messaging;
using NetworkPlayer = WatStudios.DeepestDungeon.Core.PlayerLogic.NetworkPlayer;

namespace WatStudios.DeepestDungeon.Core.AbilitySystem.Abilities
{
    [CreateAssetMenu(fileName = "ThornArmor", menuName = "ScriptableObjects/Abilities/ThornArmor")]
    public class ThornArmor : Ability
    {
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
            if (!DatabaseManager.TryGetId(statusEffect, out int statusEffectId))
                Debug.LogError("No such element in the database!");
            cooldown.SetBase();
            MessageHub.SendMessage(MessageType.AbilityCooldownStart, name);
            nPlayer.StatusEffectHandler.photonView.RPC("AddStatusEffect", Photon.Pun.RpcTarget.All, statusEffectId, nPlayer.photonView.ViewID);
        }
        #endregion
    }
}