using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.EnemyLogic.FSM
{
    public abstract class Decision : ScriptableObject
    {
        public abstract bool Decide(EnemyEntity enemy);
    }
}