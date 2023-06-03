using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.EnemyLogic.FSM
{
    [CreateAssetMenu(fileName = "WalkingDroneChase", menuName = "ScriptableObjects/AI/Actions/WalkingDrone/Chase")]
    public class WalkingDroneChase : Action
    {
        public override void Act(EnemyEntity enemy)
        {
            enemy.NavMeshAgent.SetDestination(enemy.TargetPlayer.transform.position);
            var originalForward = enemy.transform.forward;
            float turnAngle = 0.0f;
            if (enemy.NavMeshAgent.remainingDistance <= enemy.NavMeshAgent.stoppingDistance && !enemy.NavMeshAgent.pathPending)
            {
                enemy.Animator.SetFloat("Forward", 0);

                enemy.transform.LookAt(enemy.TargetPlayer.transform.position);
                turnAngle = Mathf.Abs(Vector3.SignedAngle(enemy.transform.forward, originalForward, Vector3.up));
                if (turnAngle >= 3)
                    enemy.Animator.SetFloat("Sidewards", turnAngle);
                else
                    enemy.Animator.SetFloat("Sidewards", 0);
                return;
            }

            enemy.Animator.speed = enemy.CurrentStats.MoveSpeed.Value;
            enemy.Animator.SetBool("Attack", false);

            originalForward = enemy.transform.forward;
            enemy.NavMeshAgent.Move(Vector3.zero);

            turnAngle = Mathf.Abs(Vector3.SignedAngle(enemy.transform.forward, originalForward, Vector3.up)) / 180.0f;
            enemy.Animator.SetFloat("Forward", 1);
            enemy.Animator.SetFloat("Sidewards", turnAngle);
        }
    }
}