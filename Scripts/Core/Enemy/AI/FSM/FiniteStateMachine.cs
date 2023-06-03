using Photon.Pun;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.EnemyLogic.FSM
{
    public class FiniteStateMachine : EnemyBehaviour, IPunObservable
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private FSMConfig _fsmConfig;
        [SerializeField] private int _scheduleID;
#pragma warning restore 649
        #endregion

        #region Private Fields
        private State _currentState;
        private int _currentStateId;
        #endregion

        #region Properties
        internal int CurrentStateId => _currentStateId;
        internal State CurrentState => _currentState;
        #endregion

        #region MonoBehaviour Callbacks
        internal override void Start()
        {
            _currentState = _fsmConfig.InitialState;
            DatabaseManager.TryGetId(CurrentState, out _currentStateId);
        }

        internal override void Tick()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            var nextState = CheckTransitions();

            if (nextState == CurrentState)
                _currentState.Execute(owner);
            else
                SwitchToNewState(nextState);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Checks if the FSM needs to transition to a new state
        /// </summary>
        /// <returns>The State to transition into</returns>
        private State CheckTransitions()
        {
            if (_fsmConfig.TryGetTransition(_currentState, out Transition[] transitions))
            {
                foreach (var transition in transitions)
                {
                    int result = 0;
                    foreach (var condition in transition.Conditions)
                    {
                        if (condition.Check(owner))
                            result++;
                    }

                    switch (transition.ConditionTrueAmount)
                    {
                        case ConditionTrueAmount.Any:
                            if (result > 0)
                                return transition.ToState;
                            break;
                        case ConditionTrueAmount.All:
                            if (result == transition.Conditions.Count)
                                return transition.ToState;
                            break;
                        default:
                            break;
                    }
                    if (result == transition.Conditions.Count)
                        return transition.ToState;
                }
                return _currentState;
            }
            else
            {
                Debug.LogError($"No Transitions set for current state: {_currentState}");
                return _currentState;
            }
        }

        /// <summary>
        /// Sets the current state of the FSM to <paramref name="nextState"/>
        /// </summary>
        /// <param name="nextState">Next state of the FSM</param>
        private void SwitchToNewState(State nextState)
        {
            _currentState = nextState;
        }
        #endregion

        #region IPunObservable Implementation
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (stream.IsWriting)
                {
                    stream.SendNext(CurrentStateId);
                }
            }
            else
            {
                var stateId = (int)stream.ReceiveNext();
                if (CurrentStateId != stateId)
                {
                    if (DatabaseManager.TryGetElement(stateId, out State newState))
                        SwitchToNewState(newState);
                    else
                        Debug.LogError($"No state with id {stateId} in the database!");
                }
            }
        }
        #endregion
    }
}