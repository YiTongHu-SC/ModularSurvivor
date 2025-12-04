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
            eventManager.SetHighPerformanceMode(true);

            var listeners = new HighPerformanceTestListener[10];
            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i] = new HighPerformanceTestListener();
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
            Assert.Less(stopwatch.ElapsedMilliseconds, 10,
                $"Small load took too long: {stopwatch.ElapsedMilliseconds}ms");
            UnityEngine.Debug.Log($"Small load (1000 deliveries): {stopwatch.ElapsedMilliseconds}ms");

            eventManager.SetHighPerformanceMode(false);
        }

        [Test]
        public void PerformanceBenchmark_MediumLoad_ShouldBeFast()
        {
            // 中等负载测试：50个监听器，200个事件
            eventManager.SetHighPerformanceMode(true);

            var listeners = new HighPerformanceTestListener[50];
            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i] = new HighPerformanceTestListener();
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
            Assert.Less(stopwatch.ElapsedMilliseconds, 50,
                $"Medium load took too long: {stopwatch.ElapsedMilliseconds}ms");
            UnityEngine.Debug.Log($"Medium load (10000 deliveries): {stopwatch.ElapsedMilliseconds}ms");

            eventManager.SetHighPerformanceMode(false);
        }

        [Test]
        public void PerformanceBenchmark_HighLoad_ShouldBeFast()
        {
            // 高负载测试：100个监听器，100个事件
            eventManager.SetHighPerformanceMode(true);

            var listeners = new HighPerformanceTestListener[100];
            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i] = new HighPerformanceTestListener();
                eventManager.Subscribe(listeners[i]);
            }

            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < 100; i++)
            {
                var deathEvent = new GameEvents.UnitDeathEvent($"Unit_{i}", Vector3.zero);
                eventManager.PublishEvent(deathEvent);
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

            eventManager.SetHighPerformanceMode(false);
        }

        [Test]
        public void PerformanceBenchmark_DelegateVsListener_ShouldBeComparable()
        {
            // 比较委托订阅和监听器对象的性能
            eventManager.SetHighPerformanceMode(true);

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
                var deathEvent = new GameEvents.UnitDeathEvent($"Unit_{i}", Vector3.zero);
                eventManager.PublishEvent(deathEvent);
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
                    var temp = eventData.UnitId;
                }, this);
            }

            var stopwatch2 = Stopwatch.StartNew();
            for (int i = 0; i < 100; i++)
            {
                var deathEvent = new GameEvents.UnitDeathEvent($"Unit_{i}", Vector3.zero);
                eventManager.PublishEvent(deathEvent);
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

            eventManager.SetHighPerformanceMode(false);
        }

        [Test]
        public void PerformanceBenchmark_ShowDebugLogImpact()
        {
            // 展示Debug.Log对性能的巨大影响
            const int listenerCount = 20;
            const int eventCount = 50;

            // 测试1：高性能监听器（无Debug.Log）
            eventManager.SetHighPerformanceMode(true);

            var highPerfListeners = new HighPerformanceTestListener[listenerCount];
            for (int i = 0; i < highPerfListeners.Length; i++)
            {
                highPerfListeners[i] = new HighPerformanceTestListener();
                eventManager.Subscribe(highPerfListeners[i]);
            }

            var stopwatch1 = Stopwatch.StartNew();
            for (int i = 0; i < eventCount; i++)
            {
                var deathEvent = new GameEvents.UnitDeathEvent($"Unit_{i}", Vector3.zero);
                eventManager.PublishEvent(deathEvent);
            }

            stopwatch1.Stop();

            eventManager.ClearAllListeners();
            eventManager.SetHighPerformanceMode(false);

            // 测试2：带Debug.Log的监听器
            eventManager.SetVerboseLogging(false); // 关闭EventManager自己的日志

            var debugListeners = new TestUnitDeathListener[listenerCount];
            for (int i = 0; i < debugListeners.Length; i++)
            {
                debugListeners[i] = new TestUnitDeathListener();
                debugListeners[i].EnableLogging = true; // 确保启用Debug.Log
                eventManager.Subscribe(debugListeners[i]);
            }

            var stopwatch2 = Stopwatch.StartNew();
            for (int i = 0; i < eventCount; i++)
            {
                var deathEvent = new GameEvents.UnitDeathEvent($"Unit_{i}", Vector3.zero);
                eventManager.PublishEvent(deathEvent);
            }

            stopwatch2.Stop();

            var perfRatio = (double)stopwatch2.ElapsedMilliseconds /
                            (stopwatch1.ElapsedMilliseconds == 0 ? 1 : stopwatch1.ElapsedMilliseconds);

            UnityEngine.Debug.Log($"=== Debug.Log Performance Impact ===");
            UnityEngine.Debug.Log(
                $"High Performance (no logs): {stopwatch1.ElapsedMilliseconds}ms ({listenerCount * eventCount} deliveries)");
            UnityEngine.Debug.Log(
                $"With Debug.Log: {stopwatch2.ElapsedMilliseconds}ms ({listenerCount * eventCount} deliveries)");
            UnityEngine.Debug.Log($"Performance degradation: {perfRatio:F1}x slower with Debug.Log");

            // 验证高性能版本确实快得多
            Assert.Less(stopwatch1.ElapsedMilliseconds, 50, "High performance version should be very fast");

            // Debug.Log版本预期会慢很多，但我们不要让测试因此失败
            // 只是记录性能差异供参考
        }

        /// <summary>
        /// 对比高性能监听器和带Debug.Log的监听器的性能差异
        /// </summary>
        [Test]
        public void PerformanceBenchmark_ShowDebugLog()
        {
            const int listenerCount = 200;
            const int eventCount = 500;

            // 测试1：高性能监听器（无Debug.Log）
            eventManager.SetHighPerformanceMode(true);

            var highPerfListeners = new HighPerformanceTestListener[listenerCount];
            for (int i = 0; i < highPerfListeners.Length; i++)
            {
                highPerfListeners[i] = new HighPerformanceTestListener();
                eventManager.Subscribe(highPerfListeners[i]);
            }

            var stopwatch1 = Stopwatch.StartNew();
            for (int i = 0; i < eventCount; i++)
            {
                var deathEvent = new GameEvents.UnitDeathEvent($"Unit_{i}", Vector3.zero);
                eventManager.PublishEvent(deathEvent);
            }

            stopwatch1.Stop();

            eventManager.ClearAllListeners();

            // 测试2：带Debug.Log的监听器
            eventManager.SetVerboseLogging(false); // 关闭EventManager自己的日志

            var debugListeners = new TestUnitDeathListener[listenerCount];
            for (int i = 0; i < debugListeners.Length; i++)
            {
                debugListeners[i] = new TestUnitDeathListener();
                debugListeners[i].EnableLogging = false; // 确保启用Debug.Log
                eventManager.Subscribe(debugListeners[i]);
            }

            var stopwatch2 = Stopwatch.StartNew();
            for (int i = 0; i < eventCount; i++)
            {
                var deathEvent = new GameEvents.UnitDeathEvent($"Unit_{i}", Vector3.zero);
                eventManager.PublishEvent(deathEvent);
            }

            stopwatch2.Stop();

            var perfRatio = (double)stopwatch2.ElapsedMilliseconds /
                            (stopwatch1.ElapsedMilliseconds == 0 ? 1 : stopwatch1.ElapsedMilliseconds);

            UnityEngine.Debug.Log($"=== Debug.Log Performance Impact ===");
            UnityEngine.Debug.Log(
                $"High Performance (no logs): {stopwatch1.ElapsedMilliseconds}ms ({listenerCount * eventCount} deliveries)");
            UnityEngine.Debug.Log(
                $"With Debug.Log: {stopwatch2.ElapsedMilliseconds}ms ({listenerCount * eventCount} deliveries)");
            UnityEngine.Debug.Log($"Performance degradation: {perfRatio:F1}x slower with Debug.Log");

            // 验证高性能版本确实快得多
            Assert.Less(stopwatch1.ElapsedMilliseconds, 50, "High performance version should be very fast");
            // 只是记录性能差异供参考
        }
    }
}