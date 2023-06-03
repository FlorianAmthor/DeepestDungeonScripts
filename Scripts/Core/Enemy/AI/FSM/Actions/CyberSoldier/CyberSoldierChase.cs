using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.EnemyLogic.FSM
{
    [CreateAssetMenu(fileName = "CyberSoldierChase", menuName = "ScriptableObjects/AI/Actions/CyberSoldier/Chase")]
    public class CyberSoldierChase : Action
    {
        public override void Act(EnemyEntity enemy)
        {
            enemy.NavMeshAgent.SetDestination(enemy.TargetPlayer.transform.position);
            enemy.Animator.speed = enemy.CurrentStats.MoveSpeed.Value;
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
                    enemy.Animator.SetFloat("Turn", 0);
                return;
            }
            enemy.Animator.SetBool("Attacking", false);

            originalForward = enemy.transform.forward;
            enemy.NavMeshAgent.Move(Vector3.zero);

            turnAngle = Mathf.Abs(Vector3.SignedAngle(enemy.transform.forward, originalForward, Vector3.up)) / 180.0f;
            enemy.Animator.SetFloat("Forward", 1);
            enemy.Animator.SetFloat("Turn", turnAngle);
        }
    }
}