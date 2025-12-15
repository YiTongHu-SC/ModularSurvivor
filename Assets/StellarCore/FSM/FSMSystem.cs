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
        public TE CurrentStateID { get; private set; }

        private FsmState<T, TE, TT> CurrentState { get; set; }

        public bool IsInitialized => CurrentState != null;

        private readonly T _context;

        public StateMachine(T context)
        {
            _context = context;
        }

        public bool AddState(FsmState<T, TE, TT> state)
        {
            if (state == null)
            {
                Debug.LogError("FSM ERROR: Null reference is not allowed");
                return false;
            }

            if (_statesTable.ContainsKey(state.Id))
            {
                Debug.LogError("FSM ERROR: Impossible to add state " + state.Id.ToString() +
                               " because state has already been added");
                return false;
            }

            state.Context = _context;
            _statesTable.Add(state.Id, state);
            return true;
        }

        public bool DeleteState(TE id)
        {
            if (!_statesTable.ContainsKey(id))
            {
                Debug.LogError("FSM ERROR: Impossible to delete state " + id.ToString() +
                               ". It was not on the dict of states");
                return false;
            }

            if (CurrentState != null && CurrentState.Id.Equals(id))
            {
                Debug.LogError("FSM ERROR: Cannot delete current state " + id.ToString() +
                               ". Please transition to another state first");
                return false;
            }

            _statesTable.Remove(id);
            return true;
        }

        public bool PerformTransition(TT trans)
        {
            if (CurrentState == null)
            {
                Debug.LogError("FSM ERROR: Current state is null. Cannot perform transition");
                return false;
            }

            if (!CurrentState.Contains(trans))
            {
                return false;
            }

            TE nextStateId = CurrentState.GetTargetState(trans);

            if (!_statesTable.TryGetValue(nextStateId, out var nextState))
            {
                Debug.LogError("FSM ERROR: Target state " + nextStateId.ToString() +
                               " does not exist in state table");
                return false;
            }

            try
            {
                CurrentState.Exit();
                CurrentStateID = nextStateId;
                CurrentState = nextState;
                nextState.Enter();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("FSM ERROR: Exception during state transition: " + e.Message);
                return false;
            }
        }

        public bool SetCurrent(TE id)
        {
            if (!_statesTable.ContainsKey(id))
            {
                Debug.LogError("FSM ERROR: State " + id.ToString() + " does not exist in state table");
                return false;
            }

            // 如果已经是当前状态，不需要重复设置
            if (CurrentState != null && CurrentState.Id.Equals(id))
            {
                return true;
            }

            try
            {
                CurrentState?.Exit(); // 退出当前状态（如果存在）
                CurrentStateID = id;
                CurrentState = _statesTable[id];
                CurrentState.Enter();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("FSM ERROR: Exception during state initialization: " + e.Message);
                return false;
            }
        }

        public void Tick(float deltaTime = 0)
        {
            if (CurrentState == null)
            {
                Debug.LogWarning("FSM WARNING: Cannot tick - state machine is not initialized");
                return;
            }

            CurrentState.Reason(deltaTime);
            CurrentState.Act(deltaTime);
        }

        /// <summary>
        /// 检查状态是否存在
        /// </summary>
        public bool HasState(TE stateId)
        {
            return _statesTable.ContainsKey(stateId);
        }

        /// <summary>
        /// 获取所有状态ID
        /// </summary>
        public IEnumerable<TE> GetAllStateIds()
        {
            return _statesTable.Keys;
        }

        /// <summary>
        /// 强制停止状态机
        /// </summary>
        public void Stop()
        {
            if (CurrentState != null)
            {
                try
                {
                    CurrentState.Exit();
                }
                catch (Exception e)
                {
                    Debug.LogError("FSM ERROR: Exception during state machine stop: " + e.Message);
                }

                CurrentState = null;
            }
        }

        /// <summary>
        /// 重置状态机到指定状态
        /// </summary>
        public bool Reset(TE initialState)
        {
            Stop();
            return SetCurrent(initialState);
        }
    }

    public abstract class FsmState<T, TE, TT>
        where T : MonoBehaviour
        where TE : Enum
        where TT : Enum
    {
        private readonly TE _stateId;
        private readonly Dictionary<TT, TE> _map = new Dictionary<TT, TE>();

        public T Context { get; set; }

        protected FsmState(TE stateId)
        {
            _stateId = stateId;
        }

        public TE Id => _stateId;

        public bool AddTransition(TT trans, TE id)
        {
            // Since this is a Deterministic FSM,
            //   check if the current transition was already inside the map
            if (_map.TryAdd(trans, id)) return true;
            Debug.LogError("FSMState ERROR: State " + _stateId.ToString() + " already has transition " +
                           trans.ToString() +
                           "Impossible to assign to another state");
            return false;
        }

        public bool DeleteTransition(TT trans)
        {
            // Check if the pair is inside the map before deleting
            if (_map.Remove(trans))
            {
                return true;
            }

            Debug.LogError("FSMState ERROR: Transition " + trans.ToString() + " passed to " + _stateId.ToString() +
                           " was not on the state's transition list");
            return false;
        }

        public bool TryGetTargetState(TT trans, out TE targetState)
        {
            return _map.TryGetValue(trans, out targetState);
        }

        public TE GetTargetState(TT trans)
        {
            if (_map.TryGetValue(trans, out var state)) return state;
            Debug.LogError("FSMState ERROR : Transition DO NOT exist : Transition " + trans.ToString());
            return default(TE);
        }

        public abstract void Enter();
        public abstract void Exit();
        public abstract void Reason(float deltaTime = 0);
        public abstract void Act(float deltaTime = 0);

        public bool Contains(TT trans)
        {
            return _map.ContainsKey(trans);
        }
    }
}