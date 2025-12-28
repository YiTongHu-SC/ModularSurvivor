using System;
using Combat.Ability;
using Combat.Actors;
using Combat.Data;
using Combat.Effect;
using Combat.GameCamera;
using Core.Events;
using Core.GameInterface;
using StellarCore.FSM;
using StellarCore.Singleton;
using UnityEngine;
using Utils.Core;

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

    public class CombatManager : BaseInstance<CombatManager>, IManager
    {
        public RuntimeIdAllocator GlobalAllocator { get; set; }
        public DamageSystem DamageSystem { get; set; }
        public MovementSystem MovementSystem { get; set; }
        public BuffSystem BuffSystem { get; set; }
        public AbilitySystem AbilitySystem { get; set; }
        public ViewSystem ViewSystem { get; set; }
        public ActorFactory ActorFactory { get; set; }
        public EffectSystem EffectSystem { get; set; }
        public StateMachine<CombatManager, CombatState, CombatTransition> StateMachine { get; private set; }
        public CombatState CurrentState => StateMachine.CurrentStateID;
        public CombatClockData CombatClock { get; set; }
        public Actor HeroActor { get; set; }
        public bool IsInitialized { get; private set; }

        public override void Initialize()
        {
            base.Initialize();
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
            inCombatState.AddTransition(CombatTransition.PauseCombat, CombatState.Pause);
            inCombatState.AddTransition(CombatTransition.WinCombat, CombatState.Victory);
            inCombatState.AddTransition(CombatTransition.LoseCombat, CombatState.Defeat);
            pauseState.AddTransition(CombatTransition.ResumeCombat, CombatState.InCombat);
            if (!StateMachine.SetCurrent(CombatState.Init))
            {
                Debug.LogError("Failed to set Bootstrap state");
            }
            IsInitialized = true;
        }

        private void InitSystems()
        {
            // new systems
            GlobalAllocator = new RuntimeIdAllocator();
            DamageSystem = new DamageSystem();
            MovementSystem = new MovementSystem();
            BuffSystem = new BuffSystem();
            AbilitySystem = new AbilitySystem();
            ViewSystem = new ViewSystem();
            ActorFactory = new ActorFactory();
            EffectSystem = new EffectSystem();
            // Initialize systems
            GlobalAllocator.Initialize();
            ActorFactory.Initialize(GlobalAllocator);
            // EffectSystem.Initialize();
            MovementSystem.Initialize();
            ViewSystem.Initialize();
        }

        public void Tick(float deltaTime)
        {
            // 更新所有战斗系统
            if (StateMachine.CurrentStateID == CombatState.InCombat)
            {
                AbilitySystem.Tick(deltaTime);
                EffectSystem.Tick(deltaTime);
                BuffSystem.Tick(deltaTime);
                MovementSystem.Tick(deltaTime);
                CombatClock.UpdateClock(deltaTime);
            }
        }

        public void InGameEnter()
        {
            StateMachine.PerformTransition(CombatTransition.StartCombat);
        }

        public void InGamePause()
        {
            StateMachine.PerformTransition(CombatTransition.PauseCombat);
        }

        public void InGameResume()
        {
            StateMachine.PerformTransition(CombatTransition.ResumeCombat);
        }

        public void InGameVictory()
        {
            StateMachine.PerformTransition(CombatTransition.WinCombat);
        }

        public void InGameDefeat()
        {
            StateMachine.PerformTransition(CombatTransition.LoseCombat);
        }

        public void Reset()
        {
            // 清理战斗相关数据
            ActorFactory.Reset();
            AbilitySystem.Reset();
            EffectSystem.Reset();
            BuffSystem.Reset();
            MovementSystem.Reset();
            ViewSystem.Reset();
            GlobalAllocator.Reset();
            GameObjectFactory.Cleanup();
        }

        private class CombatStateInit : FsmState<CombatManager, CombatState, CombatTransition>
        {
            public CombatStateInit() : base(CombatState.Init)
            {
            }

            public override void Enter()
            {
                Context.InitSystems();
                Context.CombatClock = new CombatClockData(300f); // 默认战斗时间300秒
            }

            public override void Exit()
            {
                EventManager.Instance.Publish(new GameLoopEvents.CombatInitCompleteEvent());
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
                Debug.Log("In Combat State Entered");
            }

            public override void Exit()
            {
                Debug.Log("In Combat State Exited");
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
                Debug.Log("Pause State Entered");
            }

            public override void Exit()
            {
                Debug.Log("Pause State Exited");
            }

            public override void Reason(float deltaTime = 0)
            {
            }

            public override void Act(float deltaTime = 0)
            {
            }
        }

        private class CombatStateVictory : FsmState<CombatManager, CombatState, CombatTransition>
        {
            public CombatStateVictory() : base(CombatState.Victory)
            {
            }

            public override void Enter()
            {
                Debug.Log("Victory State Entered");
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

        private class CombatStateDefeat : FsmState<CombatManager, CombatState, CombatTransition>
        {
            public CombatStateDefeat() : base(CombatState.Defeat)
            {
            }

            public override void Enter()
            {
                Debug.Log("Defeat State Entered");
            }

            public override void Exit()
            {
                Debug.Log("Defeat State Exited");
            }

            public override void Reason(float deltaTime = 0)
            {
            }

            public override void Act(float deltaTime = 0)
            {
            }
        }
    }
}