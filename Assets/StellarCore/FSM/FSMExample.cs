using UnityEngine;

namespace StellarCore.FSM
{
    /// <summary>
    /// 状态机使用示例 - 展示改进后的状态机如何使用
    /// </summary>
    public class FsmExample : MonoBehaviour
    {
        public enum PlayerState
        {
            Idle,
            Walking,
            Running,
            Attacking
        }

        public enum PlayerTransition
        {
            StartWalk,
            StartRun,
            Stop,
            Attack,
            AttackFinish
        }

        private StateMachine<FsmExample, PlayerState, PlayerTransition> _stateMachine;

        void Start()
        {
            // 创建状态机
            _stateMachine = new StateMachine<FsmExample, PlayerState, PlayerTransition>(this);

            // 添加状态
            var idleState = new IdleState(PlayerState.Idle);
            var walkingState = new WalkingState(PlayerState.Walking);
            var runningState = new RunningState(PlayerState.Running);

            // 安全地添加状态
            if (!_stateMachine.AddState(idleState))
            {
                Debug.LogError("Failed to add idle state");
                return;
            }

            if (!_stateMachine.AddState(walkingState))
            {
                Debug.LogError("Failed to add walking state");
                return;
            }

            if (!_stateMachine.AddState(runningState))
            {
                Debug.LogError("Failed to add running state");
                return;
            }

            // 配置状态转换
            idleState.AddTransition(PlayerTransition.StartWalk, PlayerState.Walking);
            walkingState.AddTransition(PlayerTransition.Stop, PlayerState.Idle);
            walkingState.AddTransition(PlayerTransition.StartRun, PlayerState.Running);
            runningState.AddTransition(PlayerTransition.Stop, PlayerState.Idle);

            // 设置初始状态
            if (!_stateMachine.SetCurrent(PlayerState.Idle))
            {
                Debug.LogError("Failed to set initial state");
            }
        }

        void Update()
        {
            if (_stateMachine.IsInitialized)
            {
                _stateMachine.Tick(Time.deltaTime);
            }

            // 测试状态转换
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (!_stateMachine.PerformTransition(PlayerTransition.StartWalk))
                {
                    Debug.Log("Cannot start walking from current state");
                }
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                if (!_stateMachine.PerformTransition(PlayerTransition.StartRun))
                {
                    Debug.Log("Cannot start running from current state");
                }
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                if (!_stateMachine.PerformTransition(PlayerTransition.Stop))
                {
                    Debug.Log("Cannot stop from current state");
                }
            }
        }
    }

    // 示例状态实现
    public class IdleState : FsmState<FsmExample, FsmExample.PlayerState, FsmExample.PlayerTransition>
    {
        public IdleState(FsmExample.PlayerState stateId) : base(stateId) { }

        public override void Enter()
        {
            Debug.Log("Entered Idle State");
        }

        public override void Exit()
        {
            Debug.Log("Exited Idle State");
        }

        public override void Reason(float deltaTime = 0)
        {
            // 状态逻辑判断
        }

        public override void Act(float deltaTime = 0)
        {
            // 状态行为执行
        }
    }

    public class WalkingState : FsmState<FsmExample, FsmExample.PlayerState, FsmExample.PlayerTransition>
    {
        public WalkingState(FsmExample.PlayerState stateId) : base(stateId) { }

        public override void Enter()
        {
            Debug.Log("Entered Walking State");
        }

        public override void Exit()
        {
            Debug.Log("Exited Walking State");
        }

        public override void Reason(float deltaTime = 0)
        {
            // 状态逻辑判断
        }

        public override void Act(float deltaTime = 0)
        {
            // 状态行为执行
        }
    }

    public class RunningState : FsmState<FsmExample, FsmExample.PlayerState, FsmExample.PlayerTransition>
    {
        public RunningState(FsmExample.PlayerState stateId) : base(stateId) { }

        public override void Enter()
        {
            Debug.Log("Entered Running State");
        }

        public override void Exit()
        {
            Debug.Log("Exited Running State");
        }

        public override void Reason(float deltaTime = 0)
        {
            // 状态逻辑判断
        }

        public override void Act(float deltaTime = 0)
        {
            // 状态行为执行
        }
    }
}
