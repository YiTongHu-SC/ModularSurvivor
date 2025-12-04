using System.Collections;
using System.Diagnostics;
using Core.Events;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Core.Events
{
    /// <summary>
    /// EventManager性能基准测试
    /// 验证优化效果
    /// </summary>
    public class EventSystemPerformanceTests
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
        public void PerformanceBenchmark_SmallLoad_ShouldBeFast()
        {
            // 小负载测试：10个监听器，100个事件
            eventManager.SetVerboseLogging(false);
            
            var listeners = new TestUnitDeathListener[10];
            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i] = new TestUnitDeathListener();
                eventManager.Subscribe(listeners[i]);
            }

            var stopwatch = Stopwatch.StartNew();
            
            for (int i = 0; i < 100; i++)
            {
                var deathEvent = new GameEvents.UnitDeathEvent($"Unit_{i}", Vector3.zero);
                eventManager.PublishEvent(deathEvent);
            }
            
            stopwatch.Stop();
            
            // 小负载应该非常快 (1000个事件投递)
            Assert.Less(stopwatch.ElapsedMilliseconds, 50, $"Small load took too long: {stopwatch.ElapsedMilliseconds}ms");
            UnityEngine.Debug.Log($"Small load (1000 deliveries): {stopwatch.ElapsedMilliseconds}ms");
        }

        [Test]
        public void PerformanceBenchmark_MediumLoad_ShouldBeFast()
        {
            // 中等负载测试：50个监听器，200个事件
            eventManager.SetVerboseLogging(false);
            
            var listeners = new TestUnitDeathListener[50];
            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i] = new TestUnitDeathListener();
                eventManager.Subscribe(listeners[i]);
            }

            var stopwatch = Stopwatch.StartNew();
            
            for (int i = 0; i < 200; i++)
            {
                var deathEvent = new GameEvents.UnitDeathEvent($"Unit_{i}", Vector3.zero);
                eventManager.PublishEvent(deathEvent);
            }
            
            stopwatch.Stop();
            
            // 中等负载应该在合理时间内完成 (10000个事件投递)
            Assert.Less(stopwatch.ElapsedMilliseconds, 200, $"Medium load took too long: {stopwatch.ElapsedMilliseconds}ms");
            UnityEngine.Debug.Log($"Medium load (10000 deliveries): {stopwatch.ElapsedMilliseconds}ms");
        }

        [Test]
        public void PerformanceBenchmark_HighLoad_ShouldBeFast()
        {
            // 高负载测试：100个监听器，100个事件
            eventManager.SetVerboseLogging(false);
            
            var listeners = new TestUnitDeathListener[100];
            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i] = new TestUnitDeathListener();
                eventManager.Subscribe(listeners[i]);
            }

            var stopwatch = Stopwatch.StartNew();
            
            for (int i = 0; i < 100; i++)
            {
                var deathEvent = new GameEvents.UnitDeathEvent($"Unit_{i}", Vector3.zero);
                eventManager.PublishEvent(deathEvent);
            }
            
            stopwatch.Stop();
            
            // 高负载应该在500ms内完成 (10000个事件投递)
            Assert.Less(stopwatch.ElapsedMilliseconds, 500, $"High load took too long: {stopwatch.ElapsedMilliseconds}ms");
            UnityEngine.Debug.Log($"High load (10000 deliveries): {stopwatch.ElapsedMilliseconds}ms");
            
            // 验证所有监听器都收到了事件
            foreach (var listener in listeners)
            {
                Assert.IsTrue(listener.EventReceived);
            }
        }

        [Test]
        public void PerformanceBenchmark_DelegateVsListener_ShouldBeComparable()
        {
            // 比较委托订阅和监听器对象的性能
            eventManager.SetVerboseLogging(false);
            
            // 测试监听器对象
            var listeners = new TestUnitDeathListener[50];
            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i] = new TestUnitDeathListener();
                eventManager.Subscribe(listeners[i]);
            }

            var stopwatch1 = Stopwatch.StartNew();
            for (int i = 0; i < 100; i++)
            {
                var deathEvent = new GameEvents.UnitDeathEvent($"Unit_{i}", Vector3.zero);
                eventManager.PublishEvent(deathEvent);
            }
            stopwatch1.Stop();
            
            // 清理监听器
            eventManager.ClearAllListeners();
            
            // 测试委托订阅
            for (int i = 0; i < 50; i++)
            {
                eventManager.Subscribe<GameEvents.UnitDeathEvent>(eventData => {
                    // 简单处理
                }, this);
            }

            var stopwatch2 = Stopwatch.StartNew();
            for (int i = 0; i < 100; i++)
            {
                var deathEvent = new GameEvents.UnitDeathEvent($"Unit_{i}", Vector3.zero);
                eventManager.PublishEvent(deathEvent);
            }
            stopwatch2.Stop();
            
            UnityEngine.Debug.Log($"Listener objects: {stopwatch1.ElapsedMilliseconds}ms");
            UnityEngine.Debug.Log($"Delegate subscriptions: {stopwatch2.ElapsedMilliseconds}ms");
            
            // 两种方式的性能应该都在合理范围内
            Assert.Less(stopwatch1.ElapsedMilliseconds, 300);
            Assert.Less(stopwatch2.ElapsedMilliseconds, 300);
        }
    }
}
