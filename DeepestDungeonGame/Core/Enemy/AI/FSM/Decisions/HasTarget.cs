using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.EnemyLogic.FSM
{
    [CreateAssetMenu(fileName = "HasTarget", menuName = "ScriptableObjects/AI/Decisions/HasTarget")]
    public class HasTarget : Decision
    {
        public override bool Decide(EnemyEntity enemy)
        {
            return enemy.TargetPlayer != null;
        }
    }
}


