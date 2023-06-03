using UnityEngine;
using WatStudios.DeepestDungeon.Core.EnemyLogic;
using WatStudios.DeepestDungeon.Core.EnemyLogic.ThreatSystem;
using NetworkPlayer = WatStudios.DeepestDungeon.Core.PlayerLogic.NetworkPlayer;

namespace WatStudios.DeepestDungeon.Core.AbilitySystem.StatusEffects
{
    [CreateAssetMenu(fileName = "TauntEffect", menuName = "ScriptableObjects/Abilities/StatusEffects/TauntEffect")]
    public class TauntEffect : StatusEffect
    {
        #region Properties
        public override bool IsExpired(IStatusEntity entity)
        {
            return duration.Value <= 0;
        }
        #endregion

        internal override void Apply(IStatusEntity entity)
        {
            var enemyEntity = entity as EnemyEntity;
            if (enemyEntity.StatusEffectHandler.IsAfflictedBy(this))
            {
                enemyEntity.StatusEffectHandler.RemoveStatusEffect(this);
            }
            var highThreatPlayer = enemyEntity.ThreatManager.GetHighestThreatPlayer();

            enemyEntity.ThreatManager.UpdatePlayer(Source as NetworkPlayer, Priority.High);
            enemyEntity.ThreatManager.UpdatePlayer(Source as NetworkPlayer, highThreatPlayer.Value.Threat);
            duration.SetBase();
        }

        internal override void Undo(IStatusEntity entity)
        {
            var enemyEntity = entity as EnemyEntity;
            enemyEntity.ThreatManager.UpdatePlayer(Source as NetworkPlayer, Priority.Normal);
        }
    }
}