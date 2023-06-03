using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.EnemyLogic.FSM
{
    [CreateAssetMenu(fileName = "TargetInRange", menuName = "ScriptableObjects/AI/Decisions/TargetInRange")]
    public class TargetInRange : Decision
    {
        #region Public Methods
        public override bool Decide(EnemyEntity enemy)
        {
            if (!enemy.TargetPlayer)
                return false;
            return (enemy.transform.position - enemy.TargetPlayer.transform.position).magnitude <= enemy.CurrentStats.AttackRange.Value;
        }
        #endregion
    }
}


