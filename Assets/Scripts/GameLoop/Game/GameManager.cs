using System;
using System.Collections;
using Combat.Systems;
using Core.Events;
using Core.Input;
using Core.Timer;
using Core.Units;
using StellarCore.FSM;
using StellarCore.Singleton;
using UI.Framework;
using UnityEngine;
using UnityEngine.Events;
using Waves.Systems;

namespace GameLoop.Game
{
    public enum GameState
    {
        Bootstrap = 0,
        MainMenu = 1,
        Loading = 2,
        InGame = 3,
        Exiting = 4
    }

    public enum GameTransition
    {
        FinishBoot,
        FinishLoadMain,
        FinishLoadGame,
        ExitGame,
        ReturnToMenu,
        StartGame
    }

    public class GameManager : BaseInstance<GameManager>
    {
        public UnityEvent OnGameInitialized;
        private StateMachine<GameManager, GameState, GameTransition> StateMachine { get; set; }
        public GameState CurrentState => StateMachine.CurrentStateID;
        public bool Initialized { get; private set; }
        public GameLevelStruct CurrentLevelData { get; set; }

        private void Start()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            Initialized = false;
            StateMachine = new StateMachine<GameManager, GameState, GameTransition>(this);
            var bootstrapState = new GameStateBootstrap();
            var mainMenuState = new GameStateMainMenu();
            var loadingState = new GameStateLoading();
            var inGameState = new GameStateInGame();
            var exitingState = new GameStateExiting();
            // 安全地添加状态
            if (!StateMachine.AddState(bootstrapState))
            {
                Debug.LogError("Failed to add Bootstrap state");
                return;
            }

            if (!StateMachine.AddState(mainMenuState))
            {
                Debug.LogError("Failed to add MainMenu state");
                return;
            }

            if (!StateMachine.AddState(loadingState))
            {
                Debug.LogError("Failed to add Loading state");
                return;
            }

            if (!StateMachine.AddState(inGameState))
            {
                Debug.LogError("Failed to add InGame state");
                return;
            }

            if (!StateMachine.AddState(exitingState))
            {
                Debug.LogError("Failed to add Exiting state");
                return;
            }

            // set up transitions
            bootstrapState.AddTransition(GameTransition.FinishBoot, GameState.Loading);
            loadingState.AddTransition(GameTransition.FinishLoadMain, GameState.MainMenu);
            loadingState.AddTransition(GameTransition.FinishLoadGame, GameState.InGame);
            mainMenuState.AddTransition(GameTransition.StartGame, GameState.Loading);
            mainMenuState.AddTransition(GameTransition.ExitGame, GameState.Exiting);
            inGameState.AddTransition(GameTransition.ReturnToMenu, GameState.Loading);
            inGameState.AddTransition(GameTransition.ExitGame, GameState.Exiting);
            // set initial state
            if (!StateMachine.SetCurrent(GameState.Bootstrap))
            {
                Debug.LogError("Failed to set Bootstrap state");
            }
        }

        private void GameInitializeAsync()
        {
            StartCoroutine(DelayedGameInitialization());
        }

        IEnumerator DelayedGameInitialization()
        {
            new GameObject("EventManager").AddComponent<EventManager>();
            new GameObject("InputManager").AddComponent<InputManager>();
            new GameObject("TimeManager").AddComponent<TimeManager>();
            new GameObject("UnitManager").AddComponent<UnitManager>();
            new GameObject("CombatManager").AddComponent<CombatManager>();
            new GameObject("WaveManager").AddComponent<WaveManager>();
            new GameObject("MVCManager").AddComponent<MVCManager>();
            yield return null; // 等待一帧，确保所有单例都已初始化
            EventManager.Instance.Initialize();
            InputManager.Instance.Initialize();
            TimeManager.Instance.Initialize();
            UnitManager.Instance.Initialize();
            CombatManager.Instance.Initialize();
            WaveManager.Instance.Initialize();
            MVCManager.Instance.Initialize();
            yield return null;
            Initialized = true;
            EventManager.Instance.Publish(new GameEvents.GameInitializedEvent());
            OnGameInitialized?.Invoke();
        }

        private void SubscribeEvents()
        {
            EventManager.Instance.Subscribe<GameEvents.GameStartEvent>(OnGameStart);
        }

        private void UnsubscribeEvents()
        {
            EventManager.Instance.Unsubscribe<GameEvents.GameStartEvent>(OnGameStart);
        }

        private void OnGameStart(GameEvents.GameStartEvent gameStartEvent)
        {
            CurrentLevelData = new GameLevelStruct()
            {
                LevelID = gameStartEvent.LevelID,
            };
            StateMachine.PerformTransition(GameTransition.StartGame);
        }

        private void Update()
        {
            if (!StateMachine.IsInitialized) return;
            float deltaTime = Time.deltaTime;
            StateMachine.Tick(deltaTime);
            InputManager.Instance.Tick(deltaTime);
        }

        private void FixedUpdate()
        {
            float deltaTime = Time.fixedDeltaTime;
            switch (CurrentState)
            {
                case GameState.InGame:
                    WaveManager.Instance.Tick(deltaTime);
                    CombatManager.Instance.Tick(deltaTime);
                    UnitManager.Instance.Tick(deltaTime);
                    TimeManager.Instance.Tick(deltaTime);
                    break;
            }
        }

        private class GameStateBootstrap : FsmState<GameManager, GameState, GameTransition>
        {
            public GameStateBootstrap() : base(GameState.Bootstrap)
            {
            }

            public override void Enter()
            {
                Context.GameInitializeAsync();
            }

            public override void Exit()
            {
                Context.SubscribeEvents();
            }

            public override void Reason(float deltaTime = 0)
            {
                if (Context.Initialized)
                {
                    Context.StateMachine.PerformTransition(GameTransition.FinishBoot);
                }
            }

            public override void Act(float deltaTime = 0)
            {
            }
        }

        private class GameStateMainMenu : FsmState<GameManager, GameState, GameTransition>
        {
            public GameStateMainMenu() : base(GameState.MainMenu)
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

        private class GameStateLoading : FsmState<GameManager, GameState, GameTransition>
        {
            public GameStateLoading() : base(GameState.Loading)
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

        private class GameStateInGame : FsmState<GameManager, GameState, GameTransition>
        {
            public GameStateInGame() : base(GameState.InGame)
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

        private class GameStateExiting : FsmState<GameManager, GameState, GameTransition>
        {
            public GameStateExiting() : base(GameState.Exiting)
            {
            }

            public override void Enter()
            {
                Context.UnsubscribeEvents();

                // 区分Unity编辑器和打包环境的退出处理
#if UNITY_EDITOR
                // 在Unity编辑器中停止播放
                UnityEditor.EditorApplication.isPlaying = false;
#else
                // 在打包环境中退出应用程序
                Application.Quit();
#endif
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
    }
}