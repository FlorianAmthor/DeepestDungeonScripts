using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.EnemyLogic.FSM
{
    [CreateAssetMenu(fileName = "CanAttack", menuName = "ScriptableObjects/AI/Decisions/CanAttack")]
    public class CanAttack : Decision
    {
        public override bool Decide(EnemyEntity enemy)
        {
            return enemy.CurrentStats.LastTimeAttacked + 1.0f / enemy.CurrentStats.AttackSpeed.Value <= Time.time;
        }
    }
}