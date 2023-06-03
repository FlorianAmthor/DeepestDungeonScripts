using UnityEngine;
using WatStudios.DeepestDungeon.Core.EnemyLogic;
using WatStudios.DeepestDungeon.Core.Utiliy.Areas;
using WatStudios.DeepestDungeon.Messaging;
using NetworkPlayer = WatStudios.DeepestDungeon.Core.PlayerLogic.NetworkPlayer;

namespace WatStudios.DeepestDungeon.Core.AbilitySystem.Abilities
{

    [CreateAssetMenu(fileName = "Flight", menuName = "ScriptableObjects/Abilities/Flight")]
    public class Flight : Ability
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private float _jumpHeight;
        [SerializeField] private ForceMode _forceMode;
        [SerializeField] private float _jumpRange;
        [SerializeField] private float _rootDuration;
        [SerializeField] private SphereArea _circleArea;
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
            if (!nPlayer.IsGrounded)
                IsUsable = false;
        }

        public override void Use(NetworkPlayer nPlayer, bool castOnSelf)
        {
            var cols = _circleArea.GetCollidingObjects(nPlayer.transform.position, nPlayer.transform.forward, afflictedObjects);

            foreach (var item in cols)
            {
                var enemy = item.GetComponent<EnemyEntity>();
                enemy.photonView.RPC("StartRoot", Photon.Pun.RpcTarget.MasterClient, true);
            }
            var backwards = nPlayer.transform.forward * -1;
            backwards.Normalize();
            backwards *= _jumpRange;
            backwards.y = _jumpHeight;
            nPlayer.GetComponent<Rigidbody>().AddForce(backwards, _forceMode);
            cooldown.SetBase();
            MessageHub.SendMessage(MessageType.AbilityCooldownStart, name);
            nPlayer.AllowMovementInput = false;
        }
    }
}