using System;
using System.Collections.Generic;
using UnityEngine;

namespace StellarCore.FSM
{
    /// <summary>
    /// 状态机实现
    /// </summary>
    /// /// <author> HuYiTong</author>
    /// <team>MatrixPlay</team>
    public class StateMachine<T, TE, TT>
        where T : MonoBehaviour
        where TE : Enum
        where TT : Enum
    {
        private readonly Dictionary<TE, FsmState<T, TE, TT>> _statesTable = new Dictionary<TE, FsmState<T, TE, TT>>();
        private TE _currentStateID;
        private FsmState<T, TE, TT> _currentState;
        public TE CurrentStateID => _currentStateID;
        public FsmState<T, TE, TT> CurrentState => _currentState;

        private T _context;

        public StateMachine(T context)
        {
            _context = context;
        }

        public void AddState(FsmState<T, TE, TT> state)
        {
            if (state == null)
            {
                Debug.LogError("FSM ERROR: Null reference is not allowed");
            }

            if (_statesTable.ContainsKey(state.Id))
            {
                Debug.LogError("FSM ERROR: Impossible to add state " + state.Id.ToString() +
                               " because state has already been added");
            }

            state.Context = _context;
            _statesTable.Add(state.Id, state);
        }

        public void DeleteState(TE id)
        {
            if (_statesTable.ContainsKey(id))
            {
                _statesTable.Remove(id);
            }
            else
            {
                Debug.LogError("FSM ERROR: Impossible to delete state " + id.ToString() +
                               ". It was not on the dict of states");
            }
        }

        public void PerformTransition(TT trans)
        {
            if (!_currentState.Contains(trans))
            {
                return;
            }

            _currentState.Exit();
            TE nextStateId = _currentState.GetTargetState(trans);
            var nextState = _statesTable[nextStateId];
            _currentStateID = nextStateId;
            _currentState = nextState;
            nextState.Enter();
        }

        public void SetCurrent(TE id)
        {
            _currentStateID = id;
            _currentState = _statesTable[id];
            _currentState.Enter();
        }

        public void Tick(float deltaTime = 0)
        {
            _currentState.Reason(deltaTime);
            _currentState.Act(deltaTime);
        }
    }

    public abstract class FsmState<T, TE, TT>
        where T : MonoBehaviour
        where TE : Enum
        where TT : Enum
    {
        private readonly TE _stateId;
        protected Dictionary<TT, TE> Map = new Dictionary<TT, TE>();

        public T Context { get; set; }

        protected FsmState(TE stateId)
        {
            _stateId = stateId;
        }

        public TE Id => _stateId;

        public void AddTransition(TT trans, TE id)
        {
            // Since this is a Deterministic FSM,
            //   check if the current transition was already inside the map
            if (Map.ContainsKey(trans))
            {
                Debug.LogError("FSMState ERROR: State " + _stateId.ToString() + " already has transition " +
                               trans.ToString() +
                               "Impossible to assign to another state");
                return;
            }

            Map.Add(trans, id);
        }

        public void DeleteTransition(TT trans)
        {
            // Check if the pair is inside the map before deleting
            if (Map.ContainsKey(trans))
            {
                Map.Remove(trans);
                return;
            }

            Debug.LogError("FSMState ERROR: Transition " + trans.ToString() + " passed to " + _stateId.ToString() +
                           " was not on the state's transition list");
        }

        public TE GetTargetState(TT trans)
        {
            if (!Map.ContainsKey(trans))
            {
                Debug.LogError("FSMState ERROR : Transition DO NOT exist : Transition " + trans.ToString());
            }

            return Map[trans];
        }

        public abstract void Enter();
        public abstract void Exit();
        public abstract void Reason(float deltaTime = 0);
        public abstract void Act(float deltaTime = 0);

        public bool Contains(TT trans)
        {
            return Map.ContainsKey(trans);
        }
    }
}