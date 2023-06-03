using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.AbilitySystem.StatusEffects
{
    [CreateAssetMenu(fileName = "ThornArmorEffect", menuName = "ScriptableObjects/Abilities/StatusEffects/ThornArmorEffect")]
    public class ThornArmorEffect : StatusEffect
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private int _amountReflected;
#pragma warning restore 649
        #endregion

        #region Properties
        public int AmountReflected => _amountReflected;

        public override bool IsExpired(IStatusEntity entity)
        {
            return duration.Value <= 0;
        }
        #endregion

        internal override void Apply(IStatusEntity entity) { }

        internal override void Undo(IStatusEntity entity) { }
    }
}