using System;
using System.Collections.Generic;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.EnemyLogic.FSM
{
    [CreateAssetMenu(fileName = "newFSMConfig", menuName = "ScriptableObjects/AI/FSM Config")]
    public class FSMConfig : ScriptableObject
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private State _initialState;
        [SerializeField] private Transition[] _transitions;
#pragma warning restore 649
        #endregion

        #region Private Fields
        private Dictionary<State, Transition[]> _stateTransitions;
        #endregion

        #region Properties
        public State InitialState => _initialState;
        #endregion

        #region MonoBehaviour Callbacks
        private void OnEnable()
        {
            InitTransitionDictionary();
        }
        #endregion

        #region Private Methods
        private void InitTransitionDictionary()
        {
            _stateTransitions = new Dictionary<State, Transition[]>();
            foreach (var transition in _transitions)
            {
                if (_stateTransitions.ContainsKey(transition.FromState))
                {
                    Transition[] t1 = _stateTransitions[transition.FromState];
                    Array.Resize(ref t1, t1.Length + 1);
                    t1[t1.Length - 1] = transition;
                    _stateTransitions[transition.FromState] = t1;
                }
                else
                    _stateTransitions.Add(transition.FromState, new Transition[] { transition });
            }
        }
        #endregion

        #region Public Methods
        public bool TryGetTransition(State inState, out Transition[] transitions)
        {
            return _stateTransitions.TryGetValue(inState, out transitions);
        }
        #endregion
    }
}