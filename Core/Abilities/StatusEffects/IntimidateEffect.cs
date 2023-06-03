using UnityEngine;
using WatStudios.DeepestDungeon.Core.EnemyLogic;
using NetworkPlayer = WatStudios.DeepestDungeon.Core.PlayerLogic.NetworkPlayer;

namespace WatStudios.DeepestDungeon.Core.AbilitySystem.StatusEffects
{

    [CreateAssetMenu(fileName = "IntimidateEffect", menuName = "ScriptableObjects/Abilities/StatusEffects/IntimidateEffect")]
    public class IntimidateEffect : StatusEffect
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField, Range(0, 1)] private float _additionalDamageTaken;
        [SerializeField] private int _threatPerApply;
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
            var enemyEntity = entity as EnemyEntity;
            if (!enemyEntity.StatusEffectHandler.IsAfflictedBy(this))
                enemyEntity.CurrentStats.DamageTakenMultiplier.Increase(_additionalDamageTaken);
            enemyEntity.ThreatManager.UpdatePlayer(Source as NetworkPlayer, _threatPerApply);
        }

        internal override void Undo(IStatusEntity entity)
        {
            var enemyEntity = entity as EnemyEntity;
            enemyEntity.CurrentStats.DamageTakenMultiplier.Reduce(_additionalDamageTaken);
        }
    }
}