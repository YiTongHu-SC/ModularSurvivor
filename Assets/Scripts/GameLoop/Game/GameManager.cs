using System;
using System.Collections;
using Combat.Systems;
using Core.Assets;
using Core.Events;
using Core.Input;
using Core.Timer;
using Core.Units;
using GameLoop.Config;
using StellarCore.FSM;
using StellarCore.Singleton;
using UI.Config;
using UI.Framework;
using UnityEngine;
using UnityEngine.Events;
using Waves.Systems;

namespace GameLoop.Game
{
    [Serializable]
    public enum GameState
    {
        Bootstrap = 0,
        MainMenu = 1,
        Loading = 2,
        InGame = 3,
        Exiting = 4
    }

    [Serializable]
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
        public GlobalConfig GlobalConfig;
        public UIConfig UIConfig;
        public UnityEvent OnGameInitialized;
        public Transform SystemRootSocket;
        public GameState ShowGameState;
        private StateMachine<GameManager, GameState, GameTransition> StateMachine { get; set; }
        public GameState CurrentState => StateMachine.CurrentStateID;
        public bool Initialized { get; private set; }
        private LoadSceneStruct CurrentLevelData { get; set; }
        private LoadSceneType CurrentLoadSceneType { get; set; }
        public AssetSystem AssetSystem { get; private set; }
        public MemoryMaintenanceService MemoryMaintenanceServiceInstance { get; private set; }
        private SceneLoader _sceneLoader;

        protected override void Awake()
        {
            base.Awake();
            if (SystemRootSocket)
            {
                SystemRootSocket.gameObject.SetActive(false);
            }
        }

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

        private void GameBootAsync()
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
            new GameObject("MVCManager").AddComponent<MvcManager>();
            yield return null; // 等待一帧，确保所有单例都已初始化
            EventManager.Instance.Initialize();
            InputManager.Instance.Initialize();
            TimeManager.Instance.Initialize();
            UnitManager.Instance.Initialize();
            CombatManager.Instance.Initialize();
            WaveManager.Instance.Initialize();
            MvcManager.Instance.Initialize(UIConfig);
            yield return null;
            if (SystemRootSocket)
            {
                SystemRootSocket.gameObject.SetActive(true);
            }

            yield return null;
            // 初始化资源系统

            if (GlobalConfig && GlobalConfig.AssetCatalog)
            {
                AssetSystem = new AssetSystem(GlobalConfig.AssetCatalog);
                if (GlobalConfig.CreateMemoryMaintenanceService)
                {
                    CreateMemoryMaintenanceService();
                }
            }
            else
            {
                Debug.LogError("GlobalConfig or AssetCatalog is not assigned!");
            }

            var loadDebugAssetsTask = AssetSystem.LoadManifestAsync(GlobalConfig.DebugManifest, AssetsScopeLabel.Debug);
            yield return new WaitUntil(() => loadDebugAssetsTask.IsCompleted);
            if (loadDebugAssetsTask.IsFaulted)
            {
                Debug.LogError($"Failed to load debug assets: {loadDebugAssetsTask.Exception}");
            }

            // 初始化场景加载器
            _sceneLoader = new GameObject("SceneLoader").AddComponent<SceneLoader>();
            DontDestroyOnLoad(_sceneLoader.gameObject);
            _sceneLoader.Initialize(GlobalConfig.SystemSceneName,
                GlobalConfig.LoadingSceneName,
                GlobalConfig.SceneMap,
                GlobalConfig.MinLoadingTime);
            // 加载完成
            Initialized = true;
            OnGameInitialized?.Invoke();
            EventManager.Instance.Publish(new GameLoopEvents.BootComplete());
            Debug.Log("Game bootstrapping completed.");
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void CreateMemoryMaintenanceService()
        {
            var go = new GameObject("MemoryMaintenanceService");
            DontDestroyOnLoad(go);
            MemoryMaintenanceServiceInstance = go.AddComponent<MemoryMaintenanceService>();
            Debug.Log("[MemoryMaintenance] Service created");
        }

        private void SubscribeEvents()
        {
            EventManager.Instance.Subscribe<GameLoopEvents.GameStartEvent>(OnGameStart);
            EventManager.Instance.Subscribe<GameLoopEvents.GameExitEvent>(OnGameExit);
            EventManager.Instance.Subscribe<GameLoopEvents.ReturnToMainMenuEvent>(ReturnToMainMenu);
        }


        private void UnsubscribeEvents()
        {
            EventManager.Instance.Unsubscribe<GameLoopEvents.GameStartEvent>(OnGameStart);
            EventManager.Instance.Unsubscribe<GameLoopEvents.GameExitEvent>(OnGameExit);
            EventManager.Instance.Unsubscribe<GameLoopEvents.ReturnToMainMenuEvent>(ReturnToMainMenu);
        }

        private void ReturnToMainMenu(GameLoopEvents.ReturnToMainMenuEvent evt)
        {
            CurrentLoadSceneType = LoadSceneType.MainMenu;
            StateMachine.PerformTransition(GameTransition.ReturnToMenu);
        }

        private void OnGameStart(GameLoopEvents.GameStartEvent gameStartEvent)
        {
            CurrentLevelData = new LoadSceneStruct()
            {
                LevelID = gameStartEvent.LevelID,
            };
            CurrentLoadSceneType = LoadSceneType.Game;
            StateMachine.PerformTransition(GameTransition.StartGame);
        }

        private void OnGameExit(GameLoopEvents.GameExitEvent gameExitEvent)
        {
            CurrentLoadSceneType = LoadSceneType.Exit;
            StateMachine.PerformTransition(GameTransition.ExitGame);
        }

        /// <summary>
        /// load main menu process
        /// </summary>
        private void LoadingMainProcess()
        {
            Debug.Log("Loading main process...");
            var loadSceneRequest = new LoadSceneRequest(GameTransition.FinishLoadMain,
                GlobalConfig.GlobalManifest);
            _sceneLoader.LoadScene(loadSceneRequest);
        }

        /// <summary>
        /// load game process
        /// </summary>
        private void LoadingGameProcess()
        {
            Debug.Log("Loading game process...");
            var loadSceneRequest = new LoadSceneRequest(GameTransition.FinishLoadGame,
                GlobalConfig.LevelManifest, AssetsScopeLabel.Level);
            _sceneLoader.LoadScene(loadSceneRequest);
        }

        public void PerformTransition(GameTransition transition)
        {
            StateMachine.PerformTransition(transition);
        }


        private void Update()
        {
            if (!StateMachine.IsInitialized) return;
            float deltaTime = Time.deltaTime;
            StateMachine.Tick(deltaTime);
            InputManager.Instance.Tick(deltaTime);
            ShowGameState = CurrentState;
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
                Context.GameBootAsync();
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
                // Simulate loading process
                switch (Context.CurrentLoadSceneType)
                {
                    case LoadSceneType.MainMenu:
                        Context.LoadingMainProcess();
                        break;
                    case LoadSceneType.Game:
                        Context.LoadingGameProcess();
                        break;
                }
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
                CombatManager.Instance.CombatClock.SetBattleClock(300f); // 设置默认战斗时间为300秒
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
                // 清理资源和单例
                Context.AssetSystem?.Dispose();
                // 取消订阅事件
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