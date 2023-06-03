using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.EnemyLogic.FSM
{
    [CreateAssetMenu(fileName = "WalkingDroneIdle", menuName = "ScriptableObjects/AI/Actions/WalkingDrone/Idle")]
    public class WalkingDroneIdle : Action
    {
        public override void Act(EnemyEntity enemy)
        {
            enemy.Animator.SetBool("Attack", false);
            enemy.Animator.SetFloat("Forward", 0);
            enemy.Animator.SetFloat("Sidewards", 0);
        }
    }
}