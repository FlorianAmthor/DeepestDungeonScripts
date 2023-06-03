using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.EnemyLogic.FSM
{
    [CreateAssetMenu(fileName = "NewState", menuName = "ScriptableObjects/AI/State")]
    public class State : ScriptableObject
    {
        [SerializeField] protected Action[] actions;

        #region Public Methods
        public void Execute(EnemyEntity enemy)
        {
            foreach (var action in actions)
            {
                action.Act(enemy);
            }
        }
        #endregion
    }
}