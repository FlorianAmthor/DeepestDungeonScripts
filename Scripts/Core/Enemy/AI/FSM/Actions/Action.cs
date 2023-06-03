using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.EnemyLogic.FSM
{
    public abstract class Action : ScriptableObject
    {
        public abstract void Act(EnemyEntity enemy);
    }
}