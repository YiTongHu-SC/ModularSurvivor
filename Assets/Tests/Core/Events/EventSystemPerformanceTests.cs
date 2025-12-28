using System.Diagnostics;
using Core.Events;
using NUnit.Framework;
using UnityEngine;

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
            var listeners = new HighPerformanceTestListener[10];
            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i] = new HighPerformanceTestListener();
                eventManager.Subscribe(listeners[i]);
            }

            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < 100; i++)
            {
                var deathEvent = new GameEvents.UnitDeathEvent(i);
                eventManager.Publish(deathEvent);
            }

            stopwatch.Stop();

            // 小负载应该非常快 (1000个事件投递)
            Assert.Less(stopwatch.ElapsedMilliseconds, 10,
                $"Small load took too long: {stopwatch.ElapsedMilliseconds}ms");
            UnityEngine.Debug.Log($"Small load (1000 deliveries): {stopwatch.ElapsedMilliseconds}ms");
        }

        [Test]
        public void PerformanceBenchmark_MediumLoad_ShouldBeFast()
        {
            var listeners = new HighPerformanceTestListener[50];
            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i] = new HighPerformanceTestListener();
                eventManager.Subscribe(listeners[i]);
            }

            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < 200; i++)
            {
                var deathEvent = new GameEvents.UnitDeathEvent(i);
                eventManager.Publish(deathEvent);
            }

            stopwatch.Stop();

            // 中等负载应该在合理时间内完成 (10000个事件投递)
            Assert.Less(stopwatch.ElapsedMilliseconds, 50,
                $"Medium load took too long: {stopwatch.ElapsedMilliseconds}ms");
            UnityEngine.Debug.Log($"Medium load (10000 deliveries): {stopwatch.ElapsedMilliseconds}ms");
        }

        [Test]
        public void PerformanceBenchmark_HighLoad_ShouldBeFast()
        {
            var listeners = new HighPerformanceTestListener[100];
            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i] = new HighPerformanceTestListener();
                eventManager.Subscribe(listeners[i]);
            }

            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < 100; i++)
            {
                var deathEvent = new GameEvents.UnitDeathEvent(i);
                eventManager.Publish(deathEvent);
            }

            stopwatch.Stop();

            // 高负载应该在很短时间内完成 (10000个事件投递)
            Assert.Less(stopwatch.ElapsedMilliseconds, 50,
                $"High load took too long: {stopwatch.ElapsedMilliseconds}ms");
            UnityEngine.Debug.Log($"High load (10000 deliveries): {stopwatch.ElapsedMilliseconds}ms");

            // 验证所有监听器都收到了事件
            foreach (var listener in listeners)
            {
                Assert.IsTrue(listener.EventReceived);
                Assert.AreEqual(100, listener.CallCount);
            }
        }

        [Test]
        public void PerformanceBenchmark_DelegateVsListener_ShouldBeComparable()
        {
            // 测试监听器对象 - 使用高性能监听器（无Debug.Log）
            var listeners = new HighPerformanceTestListener[50];
            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i] = new HighPerformanceTestListener();
                eventManager.Subscribe(listeners[i]);
            }

            var stopwatch1 = Stopwatch.StartNew();
            for (int i = 0; i < 100; i++)
            {
                var deathEvent = new GameEvents.UnitDeathEvent(i);
                eventManager.Publish(deathEvent);
            }

            stopwatch1.Stop();

            // 清理监听器
            eventManager.ClearAllListeners();

            // 测试委托订阅 - 简单的委托处理
            for (int i = 0; i < 50; i++)
            {
                eventManager.Subscribe<GameEvents.UnitDeathEvent>(eventData =>
                {
                    // 最简单的处理，模拟计数
                    var temp = eventData.RuntimeId;
                });
            }

            var stopwatch2 = Stopwatch.StartNew();
            for (int i = 0; i < 100; i++)
            {
                var deathEvent = new GameEvents.UnitDeathEvent(i);
                eventManager.Publish(deathEvent);
            }

            stopwatch2.Stop();

            UnityEngine.Debug.Log($"High Performance Listener objects: {stopwatch1.ElapsedMilliseconds}ms");
            UnityEngine.Debug.Log($"Delegate subscriptions: {stopwatch2.ElapsedMilliseconds}ms");

            // 两种方式的性能都应该非常快（使用高性能模式）
            Assert.Less(stopwatch1.ElapsedMilliseconds, 50,
                $"High performance listener objects took too long: {stopwatch1.ElapsedMilliseconds}ms");
            Assert.Less(stopwatch2.ElapsedMilliseconds, 50,
                $"Delegate subscriptions took too long: {stopwatch2.ElapsedMilliseconds}ms");

            // 验证监听器收到了所有事件
            foreach (var listener in listeners)
            {
                Assert.AreEqual(100, listener.CallCount, "Listener should have received all 100 events");
            }
        }
    }
}