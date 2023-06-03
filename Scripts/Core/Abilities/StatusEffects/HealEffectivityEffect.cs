using UnityEngine;
using WatStudios.DeepestDungeon.Messaging;
using NetworkPlayer = WatStudios.DeepestDungeon.Core.PlayerLogic.NetworkPlayer;

namespace WatStudios.DeepestDungeon.Core.AbilitySystem.StatusEffects
{
    [CreateAssetMenu(fileName = "HealEffectivityEffect", menuName = "ScriptableObjects/Abilities/StatusEffects/HealEffectivityEffect")]
    public class HealEffectivityEffect : StatusEffect
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField, Tooltip("Time in seconds after which HealEffectivity is increased again.")]
        private float _increaseEffectivityTime;
#pragma warning restore 649
        #endregion

        #region Properties
        public float IncreaseEffectivityTime => _increaseEffectivityTime;

        public override bool IsExpired(IStatusEntity entity)
        {
            NetworkPlayer nPlayer = entity as NetworkPlayer;
            return nPlayer.CurrentStats.HealEffectivity.NumOfStacks == 0;
        }

        public override int NumOfStacks(IStatusEntity entity)
        {
            var playerEntity = entity as NetworkPlayer;
            return playerEntity.CurrentStats.HealEffectivity.NumOfStacks;
        }

        #endregion

        internal override void Apply(IStatusEntity entity)
        {
            var playerEntity = entity as NetworkPlayer;
            playerEntity.CurrentStats.HealEffectivity.Reduce();
        }

        internal override void Tick(IStatusEntity entity)
        {
            //TODO: activate VFX
            var time = Time.time;
            var playerEntity = entity as NetworkPlayer;

            if (playerEntity.CurrentStats.HealEffectivity.LastTimeReduced + _increaseEffectivityTime <= time)
            {
                if (playerEntity.CurrentStats.HealEffectivity.LastTimeIncreased + _increaseEffectivityTime <= time)
                {
                    Undo(entity);
                    duration.SetBase();
                }
            }
        }

        internal override void Undo(IStatusEntity entity)
        {
            var playerEntity = entity as NetworkPlayer;
            playerEntity.CurrentStats.HealEffectivity.Increase();
            MessageHub.SendMessage(MessageType.BuffStackChange, GetType(), playerEntity.CurrentStats.HealEffectivity.NumOfStacks);
        }
    }
}