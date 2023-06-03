using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.EnemyLogic.FSM
{
    [CreateAssetMenu(fileName = "CyberSoldierIdle", menuName = "ScriptableObjects/AI/Actions/CyberSoldier/Idle")]
    public class CyberSoldierIdle : Action
    {
        public override void Act(EnemyEntity enemy)
        {
            enemy.Animator.SetBool("Attack", false);
            enemy.Animator.SetFloat("Forward", 0);
            enemy.Animator.SetFloat("Sidewards", 0);
        }
    }
}