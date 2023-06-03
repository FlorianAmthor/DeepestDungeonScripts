using UnityEngine;
using WatStudios.DeepestDungeon.Core.Attributes;
using NetworkPlayer = WatStudios.DeepestDungeon.Core.PlayerLogic.NetworkPlayer;

namespace WatStudios.DeepestDungeon.Core.AbilitySystem.StatusEffects
{
    [CreateAssetMenu(fileName = "DmgBuffEffect", menuName = "ScriptableObjects/Abilities/StatusEffects/DmgBuffEffect")]
    public class DmgBuffEffect : StatusEffect
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private int _flatDamageIncrease;
        [SerializeField, Range(0, 1)] private float _percentualDamageIncrease;
        [SerializeField, Range(0, 1)] private float _additionalDamageTaken;
#pragma warning restore 649
        #endregion

        #region Properties
        public override bool IsExpired(IStatusEntity entity)
        {
            return duration.Value <= 0;
        }
        #endregion

        internal override void Apply(IStatusEntity entity)
        {
            var playerEntity = entity as NetworkPlayer;
            playerEntity.CurrentStats.DamageModifier.IncreaseFlat(_flatDamageIncrease);
            playerEntity.CurrentStats.DamageModifier.IncreasePercentual(_percentualDamageIncrease);
            playerEntity.CurrentStats.DamageTakenMultiplier.Increase(_additionalDamageTaken);
        }

        internal override void Undo(IStatusEntity entity)
        {
            var playerEntity = entity as NetworkPlayer;
            playerEntity.CurrentStats.DamageModifier.ReduceFlat(_flatDamageIncrease);
            playerEntity.CurrentStats.DamageModifier.ReducePercentual(_percentualDamageIncrease);
            playerEntity.CurrentStats.DamageTakenMultiplier.Reduce(_additionalDamageTaken);
        }
    }
}