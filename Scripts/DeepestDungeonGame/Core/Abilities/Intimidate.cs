using UnityEngine;
using WatStudios.DeepestDungeon.Core.EnemyLogic;
using WatStudios.DeepestDungeon.Core.Utiliy.Areas;
using WatStudios.DeepestDungeon.Messaging;
using NetworkPlayer = WatStudios.DeepestDungeon.Core.PlayerLogic.NetworkPlayer;

namespace WatStudios.DeepestDungeon.Core.AbilitySystem.Abilities
{
    [CreateAssetMenu(fileName = "Intimidate", menuName = "ScriptableObjects/Abilities/Intimidate")]
    public class Intimidate : Ability
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private SphereArea _circleArea;
#pragma warning restore 649
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
            foreach (var collider in _circleArea.GetCollidingObjects(nPlayer.transform.position, nPlayer.transform.forward, afflictedObjects))
            {
                var enemy = collider.GetComponent<EnemyEntity>();
                if (!DatabaseManager.TryGetId(statusEffect, out int statusEffectId))
                    Debug.LogError("No such element in the database!");
                enemy.StatusEffectHandler.photonView.RPC("AddStatusEffect", Photon.Pun.RpcTarget.All, statusEffectId, nPlayer.photonView.ViewID);
            }
            cooldown.SetBase();
            MessageHub.SendMessage(MessageType.AbilityCooldownStart, name);
        }
        #endregion
    }
}