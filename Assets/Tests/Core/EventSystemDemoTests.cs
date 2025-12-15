using System.Collections;
using Core.Events;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Core.Events
{
    /// <summary>
    /// 简单的单位死亡监听器（用于演示测试）
    /// </summary>
    public class DemoUnitDeathListener : EventListener<GameEvents.UnitDeathEvent>
    {
        public bool EventReceived { get; private set; }
        public int LastUnitId { get; private set; }

        public override void OnEventReceived(GameEvents.UnitDeathEvent eventData)
        {
            EventReceived = true;
            LastUnitId = eventData.GUID;
            Debug.Log($"[监听器对象] Unit {eventData.GUID} died");
        }

        public void Reset()
        {
            EventReceived = false;
            LastUnitId = -1;
        }
    }

    /// <summary>
    /// 简单的波次开始监听器（用于演示测试）
    /// </summary>
    public class DemoWaveStartListener : EventListener<GameEvents.WaveStartEvent>
    {
        public bool EventReceived { get; private set; }
        public int LastWaveNumber { get; private set; }

        public override void OnEventReceived(GameEvents.WaveStartEvent eventData)
        {
            EventReceived = true;
            LastWaveNumber = eventData.WaveNumber;
            Debug.Log($"[监听器对象] Wave {eventData.WaveNumber} started with {eventData.EnemyCount} enemies");
        }

        public void Reset()
        {
            EventReceived = false;
            LastWaveNumber = 0;
        }
    }

    /// <summary>
    /// EventManager双重订阅方式演示测试
    /// 这个测试演示了两种订阅方式的实际使用和交互
    /// </summary>
    public class DualSubscriptionDemoTests
    {
        private GameObject eventManagerObject;
        private DemoUnitDeathListener unitDeathListener;
        private DemoWaveStartListener waveStartListener;

        [SetUp]
        public void Setup()
        {
            // 创建EventManager实例
            eventManagerObject = new GameObject("EventManager");
            var eventManager = eventManagerObject.AddComponent<EventManager>();
            eventManager.Initialize();

            // 创建演示监听器
            unitDeathListener = new DemoUnitDeathListener();
            waveStartListener = new DemoWaveStartListener();
        }

        [TearDown]
        public void TearDown()
        {
            if (eventManagerObject != null)
            {
                Object.DestroyImmediate(eventManagerObject);
            }
        }

        [Test]
        public void DemoListenerObjectSubscription_ShouldWork()
        {
            Debug.Log("=== 监听器对象订阅演示测试 ===");

            // 订阅事件
            EventManager.Instance.Subscribe(unitDeathListener);
            EventManager.Instance.Subscribe(waveStartListener);

            // 验证订阅成功
            Assert.AreEqual(1, EventManager.Instance.GetListenerObjectCount(typeof(GameEvents.UnitDeathEvent)));
            Assert.AreEqual(1, EventManager.Instance.GetListenerObjectCount(typeof(GameEvents.WaveStartEvent)));

            Debug.Log("监听器对象已注册");
        }

        [Test]
        public void DemoDelegateSubscription_ShouldWork()
        {
            Debug.Log("=== 委托订阅演示测试 ===");

            // 用于验证的标志
            bool playerLevelUpCalled = false;
            bool uiRefreshCalled = false;
            bool waveEndCalled = false;

            // 直接委托订阅 - 简单逻辑
            EventManager.Instance.Subscribe<GameEvents.PlayerLevelUpEvent>(eventData =>
            {
                playerLevelUpCalled = true;
                Debug.Log($"[委托] 玩家升级到 {eventData.NewLevel} 级! 获得 {eventData.ExperienceGained} 经验");
            }, this);

            // Lambda表达式订阅 - 内联逻辑
            EventManager.Instance.Subscribe<GameEvents.UIRefreshEvent>(eventData =>
            {
                uiRefreshCalled = true;
                Debug.Log($"[Lambda] UI refresh for: {eventData.UIName}");
            }, this);

            // 匿名方法订阅 - 复杂逻辑
            EventManager.Instance.Subscribe<GameEvents.WaveEndEvent>(eventData =>
            {
                waveEndCalled = true;
                Debug.Log($"[Anonymous] Wave {eventData.WaveNumber} ended");
                if (eventData.IsSuccess)
                {
                    Debug.Log($"Success! Duration: {eventData.Duration}s");
                }
                else
                {
                    Debug.Log("Failed!");
                }
            }, this);

            // 验证订阅成功
            Assert.AreEqual(1, EventManager.Instance.GetSubscriberCount(typeof(GameEvents.PlayerLevelUpEvent)));
            Assert.AreEqual(1, EventManager.Instance.GetSubscriberCount(typeof(GameEvents.UIRefreshEvent)));
            Assert.AreEqual(1, EventManager.Instance.GetSubscriberCount(typeof(GameEvents.WaveEndEvent)));

            // 测试委托是否被正确调用
            EventManager.Instance.Publish(new GameEvents.PlayerLevelUpEvent(5, 1000));
            EventManager.Instance.Publish(new GameEvents.UIRefreshEvent("TestUI", null));
            EventManager.Instance.Publish(new GameEvents.WaveEndEvent(1, true, 60f));

            // 验证委托被调用
            Assert.IsTrue(playerLevelUpCalled, "PlayerLevelUp delegate was not called");
            Assert.IsTrue(uiRefreshCalled, "UIRefresh delegate was not called");
            Assert.IsTrue(waveEndCalled, "WaveEnd delegate was not called");

            Debug.Log("委托订阅已注册且测试通过");
        }

        [Test]
        public void TestMixedEventHandling_ShouldWork()
        {
            Debug.Log("=== 混合事件处理测试 ===");

            // 设置监听器对象
            EventManager.Instance.Subscribe(unitDeathListener);
            EventManager.Instance.Subscribe(waveStartListener);

            // 设置委托订阅
            bool levelUpCalled = false;
            bool uiRefreshCalled = false;
            bool waveEndCalled = false;

            EventManager.Instance.Subscribe<GameEvents.PlayerLevelUpEvent>(eventData => { levelUpCalled = true; },
                this);

            EventManager.Instance.Subscribe<GameEvents.UIRefreshEvent>(eventData => { uiRefreshCalled = true; }, this);

            EventManager.Instance.Subscribe<GameEvents.WaveEndEvent>(eventData => { waveEndCalled = true; }, this);

            // 1. 单位死亡事件 - 只有监听器对象处理
            var deathEvent = new GameEvents.UnitDeathEvent(1, 2);
            EventManager.Instance.Publish(deathEvent);
            Assert.IsTrue(unitDeathListener.EventReceived);
            Assert.AreEqual("Enemy_001", unitDeathListener.LastUnitId);

            // 2. 玩家升级事件 - 只有委托处理
            var levelUpEvent = new GameEvents.PlayerLevelUpEvent(5, 1000);
            EventManager.Instance.Publish(levelUpEvent);
            Assert.IsTrue(levelUpCalled);

            // 3. 波次开始事件 - 监听器对象处理
            var waveEvent = new GameEvents.WaveStartEvent(3, 15);
            EventManager.Instance.Publish(waveEvent);
            Assert.IsTrue(waveStartListener.EventReceived);
            Assert.AreEqual(3, waveStartListener.LastWaveNumber);

            // 4. UI刷新事件 - 只有委托处理
            var uiEvent = new GameEvents.UIRefreshEvent("HealthBar", new { CurrentHP = 80, MaxHP = 100 });
            EventManager.Instance.Publish(uiEvent);
            Assert.IsTrue(uiRefreshCalled);

            // 5. 波次结束事件 - 只有委托处理
            var waveEndEvent = new GameEvents.WaveEndEvent(3, true, 125.5f);
            EventManager.Instance.Publish(waveEndEvent);
            Assert.IsTrue(waveEndCalled);

            // 显示统计信息
            ShowSubscriptionStats();
        }

        [Test]
        public void SubscriptionStats_ShouldShowCorrectCounts()
        {
            // 设置混合订阅
            EventManager.Instance.Subscribe(unitDeathListener);
            EventManager.Instance.Subscribe<GameEvents.PlayerLevelUpEvent>(eventData => { }, this);
            EventManager.Instance.Subscribe<GameEvents.UIRefreshEvent>(eventData => { }, this);

            // 验证统计信息
            Assert.AreEqual(1, EventManager.Instance.GetListenerObjectCount(typeof(GameEvents.UnitDeathEvent)));
            Assert.AreEqual(0, EventManager.Instance.GetSubscriberCount(typeof(GameEvents.UnitDeathEvent)));
            Assert.AreEqual(1, EventManager.Instance.GetListenerCount(typeof(GameEvents.UnitDeathEvent)));

            Assert.AreEqual(0, EventManager.Instance.GetListenerObjectCount(typeof(GameEvents.PlayerLevelUpEvent)));
            Assert.AreEqual(1, EventManager.Instance.GetSubscriberCount(typeof(GameEvents.PlayerLevelUpEvent)));
            Assert.AreEqual(1, EventManager.Instance.GetListenerCount(typeof(GameEvents.PlayerLevelUpEvent)));

            ShowSubscriptionStats();
        }

        [UnityTest]
        public IEnumerator DualSubscriptionDemo_PlayModeTest()
        {
            Debug.Log("=== PlayMode双重订阅演示 ===");

            // 设置监听器对象
            EventManager.Instance.Subscribe(unitDeathListener);
            EventManager.Instance.Subscribe(waveStartListener);

            // 设置委托订阅
            bool allEventsCalled = false;
            int eventCount = 0;

            EventManager.Instance.Subscribe<GameEvents.PlayerLevelUpEvent>(eventData =>
            {
                eventCount++;
                Debug.Log($"[PlayMode委托] 玩家升级到 {eventData.NewLevel} 级!");
            }, this);

            EventManager.Instance.Subscribe<GameEvents.WaveEndEvent>(eventData =>
            {
                eventCount++;
                if (eventCount >= 4) allEventsCalled = true;
                Debug.Log($"[PlayMode委托] 波次 {eventData.WaveNumber} 结束");
            }, this);

            yield return null; // 等待一帧

            for (int i = 0; i < 4; i++)
            {
                // 模拟游戏场景
                Debug.Log("1. 波次开始...");
                EventManager.Instance.Publish(new GameEvents.WaveStartEvent(1, 5));
                yield return new WaitForSeconds(0.1f);

                Debug.Log("2. 敌人死亡...");
                EventManager.Instance.Publish(new GameEvents.UnitDeathEvent(1, 2));
                yield return new WaitForSeconds(0.1f);

                Debug.Log("3. 玩家升级...");
                EventManager.Instance.Publish(new GameEvents.PlayerLevelUpEvent(2, 500));
                yield return new WaitForSeconds(0.1f);

                Debug.Log("4. 波次结束...");
                EventManager.Instance.Publish(new GameEvents.WaveEndEvent(1, true, 120.5f));
                yield return new WaitForSeconds(0.1f);
            }

            // 验证所有事件都被正确处理
            Assert.IsTrue(waveStartListener.EventReceived);
            Assert.IsTrue(unitDeathListener.EventReceived);
            Assert.IsTrue(allEventsCalled);

            Debug.Log("=== PlayMode演示完成 ===");
        }

        /// <summary>
        /// 显示订阅统计信息
        /// </summary>
        private void ShowSubscriptionStats()
        {
            Debug.Log("=== 订阅统计信息 ===");

            var eventTypes = new System.Type[]
            {
                typeof(GameEvents.UnitDeathEvent),
                typeof(GameEvents.PlayerLevelUpEvent),
                typeof(GameEvents.WaveStartEvent),
                typeof(GameEvents.UIRefreshEvent),
                typeof(GameEvents.WaveEndEvent)
            };

            foreach (var eventType in eventTypes)
            {
                int listenerCount = EventManager.Instance.GetListenerObjectCount(eventType);
                int subscriberCount = EventManager.Instance.GetSubscriberCount(eventType);
                int totalCount = EventManager.Instance.GetListenerCount(eventType);

                if (totalCount > 0)
                {
                    Debug.Log($"{eventType.Name}: {listenerCount} 监听器对象, {subscriberCount} 委托订阅, 总计 {totalCount}");
                }
            }
        }
    }
}