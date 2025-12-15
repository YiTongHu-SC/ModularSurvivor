using System.Collections;
using Core.Events;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Core.Events
{
    /// <summary>
    /// 事件系统性能和集成测试
    /// </summary>
    public class EventSystemIntegrationTests
    {
        private GameObject eventManagerObject;
        private EventManager eventManager;

        [SetUp]
        public void Setup()
        {
            eventManagerObject = new GameObject("EventManager");
            eventManager = eventManagerObject.AddComponent<EventManager>();
            eventManager.Initialize();
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
        public void EventSystem_MultipleListenersForSameEvent_ShouldNotifyAll()
        {
            // 创建多个监听同一事件的监听器
            var listener1 = new TestUnitDeathListener();
            var listener2 = new TestUnitDeathListener();
            var listener3 = new TestUnitDeathListener();

            eventManager.Subscribe(listener1);
            eventManager.Subscribe(listener2);
            eventManager.Subscribe(listener3);

            // 发布事件
            var deathEvent = new GameEvents.UnitDeathEvent(1);
            eventManager.Publish(deathEvent);

            // 验证所有监听器都收到了事件
            Assert.IsTrue(listener1.EventReceived);
            Assert.IsTrue(listener2.EventReceived);
            Assert.IsTrue(listener3.EventReceived);
            Assert.AreEqual(3, eventManager.GetListenerCount(typeof(GameEvents.UnitDeathEvent)));
        }

        [Test]
        public void EventSystem_NullEvent_ShouldHandleGracefully()
        {
            var listener = new TestUnitDeathListener();
            eventManager.Subscribe(listener);

            // 发布null事件
            eventManager.Publish(null);

            // 验证监听器没有收到事件
            Assert.IsFalse(listener.EventReceived);
        }

        [Test]
        public void EventSystem_UnsubscribeNonExistentListener_ShouldHandleGracefully()
        {
            var listener = new TestUnitDeathListener();

            // 尝试注销未注册的监听器
            Assert.DoesNotThrow(() => eventManager.Unsubscribe<GameEvents.UnitDeathEvent>(listener));
            listener = null;
            Assert.DoesNotThrow(() => eventManager.Unsubscribe(listener));
        }

        [UnityTest]
        public IEnumerator EventSystem_StressTest_ShouldHandleHighLoad()
        {
            // 启用高性能模式以获得最大性能
            eventManager.SetHighPerformanceMode(true);

            // 使用高性能监听器，无Debug.Log输出
            var listeners = new HighPerformanceTestListener[100];
            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i] = new HighPerformanceTestListener();
                eventManager.Subscribe(listeners[i]);
            }

            yield return null;

            // 发布大量事件
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            for (int i = 0; i < 100; i++)
            {
                var deathEvent = new GameEvents.UnitDeathEvent(i);
                eventManager.Publish(deathEvent);
            }

            stopwatch.Stop();

            yield return null;

            // 验证性能（现在应该能达到更好的性能）
            Assert.Less(stopwatch.ElapsedMilliseconds, 50,
                $"Event processing took too long: {stopwatch.ElapsedMilliseconds}ms");

            // 验证所有监听器都收到了最后一个事件
            foreach (var listener in listeners)
            {
                Assert.IsTrue(listener.EventReceived);
                Assert.AreEqual(100, listener.CallCount, "Listener should have received all 100 events");
            }

            Debug.Log($"Processed 10000 events (100 events × 100 listeners) in {stopwatch.ElapsedMilliseconds}ms");

            // 恢复正常模式
            eventManager.SetHighPerformanceMode(false);
            eventManager.SetVerboseLogging(true);
        }

        [UnityTest]
        public IEnumerator EventSystem_ExtremeLightweightTest_ShouldHandleVeryHighLoad()
        {
            // 极限性能测试：启用高性能模式
            eventManager.SetHighPerformanceMode(true);

            // 创建更多高性能监听器
            var listeners = new HighPerformanceTestListener[500];
            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i] = new HighPerformanceTestListener();
                eventManager.Subscribe(listeners[i]);
            }

            yield return null;

            // 发布更多事件
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            for (int i = 0; i < 200; i++)
            {
                var deathEvent = new GameEvents.UnitDeathEvent(i);
                eventManager.Publish(deathEvent);
            }

            stopwatch.Stop();

            yield return null;

            // 极限测试的性能预期
            Assert.Less(stopwatch.ElapsedMilliseconds, 300,
                $"Extreme load test failed: {stopwatch.ElapsedMilliseconds}ms for 100000 event deliveries");

            // 验证所有监听器都收到了最后一个事件
            int receivedCount = 0;
            foreach (var listener in listeners)
            {
                if (listener.EventReceived) receivedCount++;
                Assert.AreEqual(200, listener.CallCount,
                    $"Listener should have received all 200 events, but got {listener.CallCount}");
            }

            Assert.AreEqual(listeners.Length, receivedCount, "Not all listeners received events");

            Debug.Log(
                $"Extreme test: Processed 100000 events (200 events × 500 listeners) in {stopwatch.ElapsedMilliseconds}ms");

            // 恢复正常模式
            eventManager.SetHighPerformanceMode(false);
            eventManager.SetVerboseLogging(true);
        }

        [Test]
        public void EventSystem_ClearAllListeners_ShouldRemoveAllSubscriptions()
        {
            // 注册多种类型的监听器
            var unitDeathListener = new TestUnitDeathListener();
            var waveListener = new TestWaveStartListener();
            var levelUpListener = new TestPlayerLevelUpListener();

            eventManager.Subscribe(unitDeathListener);
            eventManager.Subscribe(waveListener);
            eventManager.Subscribe(levelUpListener);

            // 验证监听器已注册
            Assert.Greater(eventManager.GetListenerCount(typeof(GameEvents.UnitDeathEvent)), 0);
            Assert.Greater(eventManager.GetListenerCount(typeof(GameEvents.WaveStartEvent)), 0);
            Assert.Greater(eventManager.GetListenerCount(typeof(GameEvents.PlayerLevelUpEvent)), 0);

            // 清除所有监听器
            eventManager.ClearAllListeners();

            // 验证所有监听器都已移除
            Assert.AreEqual(0, eventManager.GetListenerCount(typeof(GameEvents.UnitDeathEvent)));
            Assert.AreEqual(0, eventManager.GetListenerCount(typeof(GameEvents.WaveStartEvent)));
            Assert.AreEqual(0, eventManager.GetListenerCount(typeof(GameEvents.PlayerLevelUpEvent)));
        }
    }
}