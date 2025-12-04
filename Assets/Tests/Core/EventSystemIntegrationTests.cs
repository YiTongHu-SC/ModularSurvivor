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
            var deathEvent = new GameEvents.UnitDeathEvent("TestUnit", Vector3.zero);
            eventManager.PublishEvent(deathEvent);
            
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
            eventManager.PublishEvent(null);
            
            // 验证监听器没有收到事件
            Assert.IsFalse(listener.EventReceived);
        }

        [Test]
        public void EventSystem_UnsubscribeNonExistentListener_ShouldHandleGracefully()
        {
            var listener = new TestUnitDeathListener();
            
            // 尝试注销未注册的监听器
            Assert.DoesNotThrow(() => eventManager.Unsubscribe(listener));
            Assert.DoesNotThrow(() => eventManager.Unsubscribe(null));
        }

        [UnityTest]
        public IEnumerator EventSystem_StressTest_ShouldHandleHighLoad()
        {
            // 关闭详细日志以提高测试性能
            eventManager.SetVerboseLogging(false);
            
            // 创建大量监听器
            var listeners = new TestUnitDeathListener[100];
            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i] = new TestUnitDeathListener();
                eventManager.Subscribe(listeners[i]);
            }
            
            yield return null;
            
            // 发布大量事件
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            for (int i = 0; i < 100; i++)
            {
                var deathEvent = new GameEvents.UnitDeathEvent($"Unit_{i}", Vector3.zero);
                eventManager.PublishEvent(deathEvent);
            }
            
            stopwatch.Stop();
            
            yield return null;
            
            // 验证性能（调整为更合理的预期时间）
            Assert.Less(stopwatch.ElapsedMilliseconds, 500, $"Event processing took too long: {stopwatch.ElapsedMilliseconds}ms");
            
            // 验证所有监听器都收到了最后一个事件
            foreach (var listener in listeners)
            {
                Assert.IsTrue(listener.EventReceived);
            }
            
            Debug.Log($"Processed 10000 events (100 events × 100 listeners) in {stopwatch.ElapsedMilliseconds}ms");
            
            // 重新启用日志（如果需要）
            eventManager.SetVerboseLogging(true);
        }

        [UnityTest]
        public IEnumerator EventSystem_ExtremeLightweightTest_ShouldHandleVeryHighLoad()
        {
            // 极限性能测试：更多监听器，更多事件
            eventManager.SetVerboseLogging(false);
            
            // 创建更多监听器
            var listeners = new TestUnitDeathListener[500];
            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i] = new TestUnitDeathListener();
                eventManager.Subscribe(listeners[i]);
            }
            
            yield return null;
            
            // 发布更多事件
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            for (int i = 0; i < 200; i++)
            {
                var deathEvent = new GameEvents.UnitDeathEvent($"Unit_{i}", Vector3.zero);
                eventManager.PublishEvent(deathEvent);
            }
            
            stopwatch.Stop();
            
            yield return null;
            
            // 极限测试的性能预期
            Assert.Less(stopwatch.ElapsedMilliseconds, 1000, $"Extreme load test failed: {stopwatch.ElapsedMilliseconds}ms for 100000 event deliveries");
            
            // 验证所有监听器都收到了最后一个事件
            int receivedCount = 0;
            foreach (var listener in listeners)
            {
                if (listener.EventReceived) receivedCount++;
            }
            
            Assert.AreEqual(listeners.Length, receivedCount, "Not all listeners received events");
            
            Debug.Log($"Extreme test: Processed 100000 events (200 events × 500 listeners) in {stopwatch.ElapsedMilliseconds}ms");
            
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
