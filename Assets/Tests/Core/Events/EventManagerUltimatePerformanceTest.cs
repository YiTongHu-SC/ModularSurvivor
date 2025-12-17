using System.Collections;
using Core.Events;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Core.Events
{
    /// <summary>
    /// 最简单的高性能测试监听器
    /// </summary>
    public class MinimalTestListener : EventListener<GameEvents.UnitDeathEvent>
    {
        public int CallCount;

        public override void OnEventReceived(GameEvents.UnitDeathEvent eventData)
        {
            CallCount++;
            // 绝对无任何其他操作，最大化性能
        }
    }

    /// <summary>
    /// EventManager极限性能验证测试
    /// </summary>
    public class EventManagerUltimatePerformanceTest
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
        public void UltimatePerformance_10x10_ShouldBeLightning()
        {
            // 超小负载：10个监听器，10个事件
            var listeners = new MinimalTestListener[10];
            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i] = new MinimalTestListener();
                eventManager.Subscribe(listeners[i]);
            }

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            for (int i = 0; i < 10; i++)
            {
                var deathEvent = new GameEvents.UnitDeathEvent(i);
                eventManager.Publish(deathEvent);
            }

            stopwatch.Stop();

            // 100个事件投递应该非常快
            Assert.Less(stopwatch.ElapsedMilliseconds, 10, $"Ultra small load took: {stopwatch.ElapsedMilliseconds}ms");

            foreach (var listener in listeners)
            {
                Assert.AreEqual(10, listener.CallCount);
            }

            Debug.Log($"Ultra small (100 deliveries): {stopwatch.ElapsedMilliseconds}ms");
        }

        [Test]
        public void UltimatePerformance_50x50_ShouldBeVeryFast()
        {
            // 小负载：50个监听器，50个事件

            var listeners = new MinimalTestListener[50];
            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i] = new MinimalTestListener();
                eventManager.Subscribe(listeners[i]);
            }

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            for (int i = 0; i < 50; i++)
            {
                var deathEvent = new GameEvents.UnitDeathEvent(i);
                eventManager.Publish(deathEvent);
            }

            stopwatch.Stop();

            // 2500个事件投递应该很快
            Assert.Less(stopwatch.ElapsedMilliseconds, 25, $"Small load took: {stopwatch.ElapsedMilliseconds}ms");

            foreach (var listener in listeners)
            {
                Assert.AreEqual(50, listener.CallCount);
            }

            Debug.Log($"Small (2500 deliveries): {stopwatch.ElapsedMilliseconds}ms");
        }

        [Test]
        public void UltimatePerformance_100x100_ShouldBeFast()
        {
            // 标准压力测试：100个监听器，100个事件
            var listeners = new MinimalTestListener[100];
            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i] = new MinimalTestListener();
                eventManager.Subscribe(listeners[i]);
            }

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            for (int i = 0; i < 100; i++)
            {
                var deathEvent = new GameEvents.UnitDeathEvent(i);
                eventManager.Publish(deathEvent);
            }

            stopwatch.Stop();

            // 10000个事件投递应该在50ms内完成
            Assert.Less(stopwatch.ElapsedMilliseconds, 50, $"Standard load took: {stopwatch.ElapsedMilliseconds}ms");

            foreach (var listener in listeners)
            {
                Assert.AreEqual(100, listener.CallCount);
            }

            Debug.Log($"Standard (10000 deliveries): {stopwatch.ElapsedMilliseconds}ms");
        }

        [UnityTest]
        public IEnumerator UltimatePerformance_FullStressTest()
        {
            // 完整压力测试：在UnityTest中运行

            var listeners = new MinimalTestListener[100];
            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i] = new MinimalTestListener();
                eventManager.Subscribe(listeners[i]);
            }

            yield return null; // 等待一帧

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            for (int i = 0; i < 100; i++)
            {
                var deathEvent = new GameEvents.UnitDeathEvent(i);
                eventManager.Publish(deathEvent);

                // 每10个事件让出一帧，模拟真实游戏场景
                if (i % 10 == 0)
                {
                    yield return null;
                }
            }

            stopwatch.Stop();

            yield return null; // 最后等待一帧

            // 在Unity协程环境下的性能要求
            Assert.Less(stopwatch.ElapsedMilliseconds, 100,
                $"Unity coroutine stress test took: {stopwatch.ElapsedMilliseconds}ms");

            foreach (var listener in listeners)
            {
                Assert.AreEqual(100, listener.CallCount);
            }

            Debug.Log($"Unity coroutine stress test (10000 deliveries): {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}