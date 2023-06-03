using UnityEngine;
using WatStudios.DeepestDungeon.Core.EnemyLogic;
using WatStudios.DeepestDungeon.Messaging;
using NetworkPlayer = WatStudios.DeepestDungeon.Core.PlayerLogic.NetworkPlayer;

namespace WatStudios.DeepestDungeon.Core.AbilitySystem.Abilities
{
    [CreateAssetMenu(fileName = "PhaseShift", menuName = "ScriptableObjects/Abilities/PhaseShift")]
    public class PhaseShift : Ability
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private float _maxRange;
        [SerializeField] private LayerMask _blockingObjects;
#pragma warning restore 649
        #endregion

        public override void Tick(NetworkPlayer nPlayer)
        {
            if (cooldown.Value > 0)
            {
                if (IsUsable)
                    IsUsable = false;
                OnCooldown();
            }
            if (cooldown.Value <= 0 && !IsUsable && nPlayer.IsGrounded)
            {
                IsUsable = true;
                MessageHub.SendMessage(MessageType.AbilityCooldownEnd, name);
            }
            //if (!nPlayer.IsGrounded)
            //    IsUsable = false;
        }

        public override void Use(NetworkPlayer nPlayer, bool castOnSelf)
        {
            foreach (var entityObj in EnemyManager.Instance.ActiveEntities.Values)
            {
                var enemy = entityObj.GetComponent<EnemyEntity>();
                enemy.ThreatManager.UpdatePlayer(nPlayer, EnemyLogic.ThreatSystem.Priority.Normal);
                enemy.ThreatManager.UpdatePlayer(nPlayer, threat: 0);
            }

            //Vector3 blinkDirection = nPlayer.transform.forward;
            //var capsuleCol = nPlayer.GetComponent<CapsuleCollider>();
            //float blinkLength = _maxRange;

            //if (Physics.CapsuleCast(nPlayer.transform.position, nPlayer.transform.position +  new Vector3(0, capsuleCol.height, 0), capsuleCol.radius, blinkDirection, out RaycastHit hit, _maxRange, _blockingObjects))
            //{
            //    blinkLength = hit.distance - capsuleCol.radius;
            //}

            //nPlayer.transform.position = nPlayer.transform.position + (blinkDirection * blinkLength);
            cooldown.SetBase();
            MessageHub.SendMessage(MessageType.AbilityCooldownStart, name);
            nPlayer.AllowMovementInput = false;
        }
    }
}