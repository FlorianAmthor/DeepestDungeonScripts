using UnityEngine;
using WatStudios.DeepestDungeon.Core.Attributes;
using NetworkPlayer = WatStudios.DeepestDungeon.Core.PlayerLogic.NetworkPlayer;

namespace WatStudios.DeepestDungeon.Core.AbilitySystem.StatusEffects
{
    [CreateAssetMenu(fileName = "ShieldEffect", menuName = "ScriptableObjects/Abilities/StatusEffects/ShieldEffect")]
    public class ShieldEffect : StatusEffect
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private ShieldPoints _shieldPoints;
#pragma warning restore 649
        #endregion

        #region Properties
        public override bool IsExpired(IStatusEntity entity)
        {
            NetworkPlayer nPlayer = entity as NetworkPlayer;
            return nPlayer.CurrentStats.ShieldPoints.Value <= 0 || duration.Value <= 0;
        }
        #endregion

        internal override void Apply(IStatusEntity entity)
        {
            var playerEntity = entity as NetworkPlayer;
            playerEntity.CurrentStats.ShieldPoints.Increase(_shieldPoints.MaxValue);
            playerEntity.CurrentStats.HealEffectivity.SetZero();
        }

        internal override void Undo(IStatusEntity entity)
        {
            var playerEntity = entity as NetworkPlayer;
            playerEntity.CurrentStats.ShieldPoints.SetZero();
        }
    }
}