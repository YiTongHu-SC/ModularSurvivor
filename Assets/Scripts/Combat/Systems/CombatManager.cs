using Combat.Actors;
using Combat.Data;
using Combat.GameCamera;
using StellarCore.FSM;
using StellarCore.Singleton;
using UnityEngine;

namespace Combat.Systems
{
    public enum CombatState
    {
        Init,
        InCombat,
        Pause,
        Victory,
        Defeat
    }

    public enum CombatTransition
    {
        StartCombat,
        PauseCombat,
        ResumeCombat,
        WinCombat,
        LoseCombat
    }

    public class CombatManager : BaseInstance<CombatManager>
    {
        public DamageSystem DamageSystem { get; set; } = new();
        public MovementSystem MovementSystem { get; set; } = new();
        public BuffSystem BuffSystem { get; set; } = new();
        public AbilitySystem AbilitySystem { get; set; } = new();
        public ViewSystem ViewSystem { get; set; } = new();
        public Actor HeroActor { get; set; }
        public ActorFactory ActorFactory { get; set; } = new();
        public CombatClockData CombatClock { get; set; }
        public StateMachine<CombatManager, CombatState, CombatTransition> StateMachine { get; private set; }
        public CameraManager CameraManager { get; set; }

        public override void Initialize()
        {
            InitializeCombatState();
        }

        private void InitializeCombatState()
        {
            StateMachine = new StateMachine<CombatManager, CombatState, CombatTransition>(this);
            var initState = new CombatStateInit();
            var inCombatState = new CombatStateInCombat();
            var pauseState = new CombatStatePause();
            var victoryState = new CombatStateVictory();
            var defeatState = new CombatStateDefeat();
            StateMachine.AddState(initState);
            StateMachine.AddState(inCombatState);
            StateMachine.AddState(pauseState);
            StateMachine.AddState(victoryState);
            StateMachine.AddState(defeatState);

            // set up transitions
            initState.AddTransition(CombatTransition.StartCombat, CombatState.InCombat);
            if (!StateMachine.SetCurrent(CombatState.Init))
            {
                Debug.LogError("Failed to set Bootstrap state");
            }
        }

        public void Tick(float deltaTime)
        {
            // 更新所有战斗系统
            if (StateMachine.CurrentStateID == CombatState.InCombat)
            {
                AbilitySystem.UpdateAbilities(deltaTime);
                BuffSystem.UpdateBuffs(deltaTime);
                MovementSystem.UpdateMovement(deltaTime);
                CombatClock.UpdateClock(deltaTime);
            }
        }

        public void InGameEnter()
        {
            StateMachine.PerformTransition(CombatTransition.StartCombat);
        }

        private class CombatStateInit : FsmState<CombatManager, CombatState, CombatTransition>
        {
            public CombatStateInit() : base(CombatState.Init)
            {
            }

            public override void Enter()
            {
                Context.AbilitySystem.Initialize();
                Context.BuffSystem.Initialize();
                Context.MovementSystem.Initialize();
                Context.ViewSystem.Initialize();
                Context.CombatClock = new CombatClockData(300f); // 默认战斗时间300秒
            }

            public override void Exit()
            {
            }

            public override void Reason(float deltaTime = 0)
            {
            }

            public override void Act(float deltaTime = 0)
            {
            }
        }

        private class CombatStateInCombat : FsmState<CombatManager, CombatState, CombatTransition>
        {
            public CombatStateInCombat() : base(CombatState.InCombat)
            {
            }

            public override void Enter()
            {
            }

            public override void Exit()
            {
            }

            public override void Reason(float deltaTime = 0)
            {
            }

            public override void Act(float deltaTime = 0)
            {
            }
        }

        private class CombatStatePause : FsmState<CombatManager, CombatState, CombatTransition>
        {
            public CombatStatePause() : base(CombatState.Pause)
            {
            }

            public override void Enter()
            {
                throw new System.NotImplementedException();
            }

            public override void Exit()
            {
                throw new System.NotImplementedException();
            }

            public override void Reason(float deltaTime = 0)
            {
                throw new System.NotImplementedException();
            }

            public override void Act(float deltaTime = 0)
            {
                throw new System.NotImplementedException();
            }
        }

        private class CombatStateVictory : FsmState<CombatManager, CombatState, CombatTransition>
        {
            public CombatStateVictory() : base(CombatState.Victory)
            {
            }

            public override void Enter()
            {
                throw new System.NotImplementedException();
            }

            public override void Exit()
            {
                throw new System.NotImplementedException();
            }

            public override void Reason(float deltaTime = 0)
            {
                throw new System.NotImplementedException();
            }

            public override void Act(float deltaTime = 0)
            {
                throw new System.NotImplementedException();
            }
        }

        private class CombatStateDefeat : FsmState<CombatManager, CombatState, CombatTransition>
        {
            public CombatStateDefeat() : base(CombatState.Defeat)
            {
            }

            public override void Enter()
            {
                throw new System.NotImplementedException();
            }

            public override void Exit()
            {
                throw new System.NotImplementedException();
            }

            public override void Reason(float deltaTime = 0)
            {
                throw new System.NotImplementedException();
            }

            public override void Act(float deltaTime = 0)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}