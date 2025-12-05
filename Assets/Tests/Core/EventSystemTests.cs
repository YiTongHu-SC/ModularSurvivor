using System.Collections;
using Core.Events;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Core.Events
{
    /// <summary>
    /// 事件系统单元测试
    /// </summary>
    public class EventSystemTests
    {
        private GameObject eventManagerObject;
        private EventManager eventManager;

        [SetUp]
        public void Setup()
        {
            // 创建EventManager实例用于测试
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

        #region 监听器对象测试

        [Test]
        public void EventManager_Initialize_ShouldNotBeNull()
        {
            // 测试EventManager初始化
            Assert.IsNotNull(eventManager);
            Assert.IsNotNull(EventManager.Instance);
        }

        [Test]
        public void EventManager_Subscribe_ShouldRegisterListener()
        {
            // 创建测试监听器
            var testListener = new TestUnitDeathListener();

            // 订阅事件
            eventManager.Subscribe(testListener);

            // 验证监听器数量
            Assert.AreEqual(1, eventManager.GetListenerObjectCount(typeof(GameEvents.UnitDeathEvent)));
            Assert.AreEqual(1, eventManager.GetListenerCount(typeof(GameEvents.UnitDeathEvent)));
        }

        [Test]
        public void EventManager_PublishEvent_ShouldNotifyListeners()
        {
            // 创建测试监听器
            var testListener = new TestUnitDeathListener();
            eventManager.Subscribe(testListener);

            // 发布事件
            var deathEvent = new GameEvents.UnitDeathEvent("TestUnit", Vector3.zero, "TestKiller");
            eventManager.PublishEvent(deathEvent);

            // 验证事件被接收
            Assert.IsTrue(testListener.EventReceived);
            Assert.AreEqual("TestUnit", testListener.LastUnitId);
        }

        [Test]
        public void EventManager_Unsubscribe_ShouldRemoveListener()
        {
            // 创建测试监听器
            var testListener = new TestUnitDeathListener();
            eventManager.Subscribe(testListener);

            // 验证监听器已注册
            Assert.AreEqual(1, eventManager.GetListenerCount(typeof(GameEvents.UnitDeathEvent)));

            // 注销监听器
            eventManager.Unsubscribe(testListener, typeof(GameEvents.UnitDeathEvent));

            // 验证监听器已移除
            Assert.AreEqual(0, eventManager.GetListenerCount(typeof(GameEvents.UnitDeathEvent)));
        }

        #endregion

        #region 委托订阅测试

        [Test]
        public void EventManager_DelegateSubscribe_ShouldRegisterCallback()
        {
            // 委托订阅
            eventManager.Subscribe<GameEvents.UnitDeathEvent>(eventData =>
            {
                // 空回调用于测试注册
            }, this);

            // 验证订阅者数量
            Assert.AreEqual(1, eventManager.GetSubscriberCount(typeof(GameEvents.UnitDeathEvent)));
            Assert.AreEqual(1, eventManager.GetListenerCount(typeof(GameEvents.UnitDeathEvent)));
        }

        [Test]
        public void EventManager_DelegatePublishEvent_ShouldNotifySubscribers()
        {
            // 委托订阅
            bool callbackInvoked = false;
            string receivedUnitId = null;

            eventManager.Subscribe<GameEvents.UnitDeathEvent>(eventData =>
            {
                callbackInvoked = true;
                receivedUnitId = eventData.UnitId;
            }, this);

            // 发布事件
            var deathEvent = new GameEvents.UnitDeathEvent("TestUnit", Vector3.zero, "TestKiller");
            eventManager.PublishEvent(deathEvent);

            // 验证事件被接收
            Assert.IsTrue(callbackInvoked);
            Assert.AreEqual("TestUnit", receivedUnitId);
        }

        [Test]
        public void EventManager_UnsubscribeAll_ShouldRemoveAllSubscriptions()
        {
            // 委托订阅多个事件
            eventManager.Subscribe<GameEvents.UnitDeathEvent>(eventData => { }, this);
            eventManager.Subscribe<GameEvents.PlayerLevelUpEvent>(eventData => { }, this);

            // 验证订阅者已注册
            Assert.AreEqual(1, eventManager.GetSubscriberCount(typeof(GameEvents.UnitDeathEvent)));
            Assert.AreEqual(1, eventManager.GetSubscriberCount(typeof(GameEvents.PlayerLevelUpEvent)));

            // 取消所有订阅
            eventManager.UnsubscribeAll(this);

            // 验证订阅者已移除
            Assert.AreEqual(0, eventManager.GetSubscriberCount(typeof(GameEvents.UnitDeathEvent)));
            Assert.AreEqual(0, eventManager.GetSubscriberCount(typeof(GameEvents.PlayerLevelUpEvent)));
        }

        #endregion

        #region 混合模式测试

        [Test]
        public void EventManager_MixedSubscription_ShouldWorkTogether()
        {
            // 监听器对象订阅
            var testListener = new TestUnitDeathListener();
            eventManager.Subscribe(testListener);

            // 委托订阅同一事件
            bool delegateInvoked = false;
            eventManager.Subscribe<GameEvents.UnitDeathEvent>(eventData => { delegateInvoked = true; }, this);

            // 验证总数量
            Assert.AreEqual(1, eventManager.GetListenerObjectCount(typeof(GameEvents.UnitDeathEvent)));
            Assert.AreEqual(1, eventManager.GetSubscriberCount(typeof(GameEvents.UnitDeathEvent)));
            Assert.AreEqual(2, eventManager.GetListenerCount(typeof(GameEvents.UnitDeathEvent)));

            // 发布事件
            var deathEvent = new GameEvents.UnitDeathEvent("TestUnit", Vector3.zero);
            eventManager.PublishEvent(deathEvent);

            // 验证两种方式都收到事件
            Assert.IsTrue(testListener.EventReceived);
            Assert.IsTrue(delegateInvoked);
        }

        [UnityTest]
        public IEnumerator EventManager_MultipleEventsWithMixedSubscription_ShouldHandleCorrectly()
        {
            // 创建多个测试监听器
            var unitDeathListener = new TestUnitDeathListener();
            var waveListener = new TestWaveStartListener();

            eventManager.Subscribe(unitDeathListener);
            eventManager.Subscribe(waveListener);

            // 委托订阅
            bool levelUpDelegateInvoked = false;
            int receivedLevel = 0;

            eventManager.Subscribe<GameEvents.PlayerLevelUpEvent>(eventData =>
            {
                levelUpDelegateInvoked = true;
                receivedLevel = eventData.NewLevel;
            }, this);

            yield return null; // 等待一帧

            // 发布不同类型的事件
            var deathEvent = new GameEvents.UnitDeathEvent("Enemy_001", Vector3.zero);
            var waveEvent = new GameEvents.WaveStartEvent(1, 5);
            var levelUpEvent = new GameEvents.PlayerLevelUpEvent(3, 500);

            eventManager.PublishEvent(deathEvent);
            eventManager.PublishEvent(waveEvent);
            eventManager.PublishEvent(levelUpEvent);

            yield return null; // 等待一帧

            // 验证各自的监听器都收到了正确的事件
            Assert.IsTrue(unitDeathListener.EventReceived);
            Assert.IsTrue(waveListener.EventReceived);
            Assert.IsTrue(levelUpDelegateInvoked);
            Assert.AreEqual("Enemy_001", unitDeathListener.LastUnitId);
            Assert.AreEqual(1, waveListener.LastWaveNumber);
            Assert.AreEqual(3, receivedLevel);
        }

        #endregion
    }
}