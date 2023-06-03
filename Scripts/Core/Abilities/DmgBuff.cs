using UnityEngine;
using WatStudios.DeepestDungeon.Messaging;
using NetworkPlayer = WatStudios.DeepestDungeon.Core.PlayerLogic.NetworkPlayer;

namespace WatStudios.DeepestDungeon.Core.AbilitySystem.Abilities
{
    [CreateAssetMenu(fileName = "DmgBuff", menuName = "ScriptableObjects/Abilities/DmgBuff")]
    public class DmgBuff : Ability
    {
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
            var target = nPlayer;
            var sfHandler = nPlayer.StatusEffectHandler;
            if (!DatabaseManager.TryGetId(statusEffect, out int statusEffectId))
                Debug.LogError("No such element in the database!");
            cooldown.SetBase();
            MessageHub.SendMessage(MessageType.AbilityCooldownStart, name);
            sfHandler.photonView.RPC("AddStatusEffect", target.PhotonPlayer, statusEffectId, nPlayer.photonView.ViewID);
        }
    }
}