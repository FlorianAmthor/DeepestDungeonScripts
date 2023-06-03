using System.Collections.Generic;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.EnemyLogic.FSM
{
    [System.Serializable]
    public class Transition
    {
        #region Exposed Privat Fields
#pragma warning disable 649
        [SerializeField] private State _fromState;
        [SerializeField] private List<Condition> _conditions;
        [SerializeField, Tooltip("Number of conditions that need to be fullfilled to switch into next state")]
        private ConditionTrueAmount _conditionTrueAmount;
        [SerializeField] private State _toState;
#pragma warning restore 649
        #endregion

        #region Properties
        public List<Condition> Conditions => _conditions;
        public ConditionTrueAmount ConditionTrueAmount => _conditionTrueAmount;
        public State FromState => _fromState;
        public State ToState => _toState;
        #endregion
    }
}