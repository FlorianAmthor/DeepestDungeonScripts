using System;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.EnemyLogic.FSM
{
    [Serializable]
    public class Condition
    {
        #region Private Exposed Fields
#pragma warning disable 649
        [SerializeField] private Decision _decision;
        [SerializeField] private BoolResult _desiredResult;
#pragma warning restore 649
        #endregion

        #region Public Methods
        public bool Check(EnemyEntity enemy)
        {
            bool desiredResult = Convert.ToBoolean((int)_desiredResult);
            return desiredResult == _decision.Decide(enemy);
        }
        #endregion
    }
}