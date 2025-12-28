using System;
using Core.Events;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

// 简单测试类来验证回调取消订阅功能
namespace Tests.Core.Events
{
    public class TestCallbackUnsubscribe
    {
        private static int callbackInvokedCount = 0;
        private EventManager eventManager;
        private GameObject eventManagerObject;

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
        public static void RunTest()
        {
            Console.WriteLine("=== 测试委托取消订阅功能 ===");

            var eventManager = EventManager.Instance;

            // 订阅事件
            eventManager.Subscribe<GameEvents.UnitDeathEvent>(OnUnitDeathEvent);
            Console.WriteLine($"订阅后的监听器数量: {eventManager.GetSubscriberCount(typeof(GameEvents.UnitDeathEvent))}");

            // 验证订阅者已注册
            Assert.AreEqual(1, eventManager.GetSubscriberCount(typeof(GameEvents.UnitDeathEvent)));
            // 发布事件测试
            eventManager.Publish(new GameEvents.UnitDeathEvent(1));
            Console.WriteLine($"事件发布后回调调用次数: {callbackInvokedCount}");

            // 验证回调已被调用
            Assert.AreEqual(1, callbackInvokedCount);

            // 取消订阅
            eventManager.Unsubscribe<GameEvents.UnitDeathEvent>(OnUnitDeathEvent);
            Console.WriteLine($"取消订阅后的监听器数量: {eventManager.GetSubscriberCount(typeof(GameEvents.UnitDeathEvent))}");

            Assert.AreEqual(0, eventManager.GetSubscriberCount(typeof(GameEvents.UnitDeathEvent)));

            // 再次发布事件测试
            callbackInvokedCount = 0; // 重置计数器
            eventManager.Publish(new GameEvents.UnitDeathEvent(1));
            Console.WriteLine($"取消订阅后事件发布，回调调用次数: {callbackInvokedCount}");
            Assert.AreEqual(0, callbackInvokedCount);

            Console.WriteLine("=== 测试完成 ===");
        }

        private static void OnUnitDeathEvent(GameEvents.UnitDeathEvent eventData)
        {
            callbackInvokedCount++;
            Console.WriteLine($"收到单位死亡事件: {eventData.RuntimeId}");
        }
    }
}