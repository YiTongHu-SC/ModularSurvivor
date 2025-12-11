using System.Collections;
using Combat.Systems;
using Core.Abstructs;
using Core.Events;
using Core.Input;
using Core.Timer;
using Core.Units;
using UnityEngine;
using UnityEngine.Events;
using Waves.Systems;

namespace GameLoop.Game
{
    public class GameManager : BaseInstance<GameManager>
    {
        private bool _initialized = false;
        public UnityEvent OnGameInitialized;

        protected override void Awake()
        {
            base.Awake();
            if (Instance == this)
            {
                Initialize();
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            _initialized = false;
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
            yield return null; // 等待一帧，确保所有单例都已初始化
            EventManager.Instance.Initialize();
            InputManager.Instance.Initialize();
            TimeManager.Instance.Initialize();
            UnitManager.Instance.Initialize();
            CombatManager.Instance.Initialize();
            WaveManager.Instance.Initialize();
            yield return null;
            _initialized = true;
            EventManager.Instance.PublishEvent(new GameEvents.GameInitializedEvent());
            OnGameInitialized?.Invoke();
        }

        private void FixedUpdate()
        {
            if (!_initialized) return;
            float deltaTime = Time.fixedDeltaTime;
            WaveManager.Instance.Tick(deltaTime);
            CombatManager.Instance.Tick(deltaTime);
            UnitManager.Instance.Tick(deltaTime);
            TimeManager.Instance.Tick(deltaTime);
        }
    }
}