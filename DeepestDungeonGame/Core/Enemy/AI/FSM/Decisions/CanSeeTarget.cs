using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.EnemyLogic.FSM
{
    [CreateAssetMenu(fileName = "CanSeeTarget", menuName = "ScriptableObjects/AI/Decisions/CanSeeTarget")]
    public class CanSeeTarget : Decision
    {
        public override bool Decide(EnemyEntity enemy)
        {
            if (enemy.TargetPlayer)
            {
                //TODO: eye position for each enemy and do a sphere cast instead of raycast
                if (Physics.Raycast(enemy.transform.position, enemy.TargetPlayer.transform.position - enemy.transform.position, out RaycastHit hit, enemy.CurrentStats.AttackRange.Value))
                {
                    if (hit.collider.CompareTag("Player"))
                        return true;
                }
            }
            return false;
        }
    }
}


