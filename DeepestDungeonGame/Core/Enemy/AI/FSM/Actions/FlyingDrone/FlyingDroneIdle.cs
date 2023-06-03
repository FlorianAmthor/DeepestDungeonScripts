using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.EnemyLogic.FSM
{
    [CreateAssetMenu(fileName = "FlyingDroneIdle", menuName = "ScriptableObjects/AI/Actions/FlyingDrone/Idle")]
    public class FlyingDroneIdle : Action
    {
        public override void Act(EnemyEntity enemy)
        {
            enemy.Animator.SetBool("Attack", false);
            enemy.Animator.SetFloat("Forward", 0);
            enemy.Animator.SetFloat("Sidewards", 0);
        }
    }
}