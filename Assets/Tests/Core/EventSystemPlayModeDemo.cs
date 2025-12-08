using System.Collections;
using Core.Events;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Core.Events
{
    /// <summary>
    /// 事件系统PlayMode测试和演示
    /// 这个测试可以在PlayMode中运行，演示事件系统的实际使用
    /// </summary>
    public class EventSystemPlayModeDemo
    {
        private GameObject eventManagerObject;
        private TestUnitDeathListener unitDeathListener;
        private TestWaveStartListener waveListener;
        private TestPlayerLevelUpListener playerLevelUpListener;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            // 创建EventManager
            eventManagerObject = new GameObject("EventManager");
            var eventManager = eventManagerObject.AddComponent<EventManager>();

            // 创建监听器实例
            unitDeathListener = new TestUnitDeathListener();
            waveListener = new TestWaveStartListener();
            playerLevelUpListener = new TestPlayerLevelUpListener();

            // 注册监听器
            EventManager.Instance.Subscribe(unitDeathListener);
            EventManager.Instance.Subscribe(waveListener);
            EventManager.Instance.Subscribe(playerLevelUpListener);

            Debug.Log("Event listeners registered for PlayMode demo.");

            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            // 注销监听器
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe(unitDeathListener);
                EventManager.Instance.Unsubscribe(waveListener);
                EventManager.Instance.Unsubscribe(playerLevelUpListener);
            }

            if (eventManagerObject != null)
            {
                Object.DestroyImmediate(eventManagerObject);
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator EventDemo_UnitDeathEvent_ShouldWork()
        {
            Debug.Log("=== Testing Unit Death Event ===");

            var deathEvent = new GameEvents.UnitDeathEvent(
                1,
                2
            );

            EventManager.Instance.PublishEvent(deathEvent);

            yield return new WaitForSeconds(0.1f);

            // 验证事件处理
            UnityEngine.Assertions.Assert.IsTrue(unitDeathListener.EventReceived, "Unit death event was not received");
            UnityEngine.Assertions.Assert.AreEqual(1, unitDeathListener.LastUnitId);

            Debug.Log("Unit Death Event test completed successfully!");
            yield return null;
        }

        [UnityTest]
        public IEnumerator EventDemo_WaveStartEvent_ShouldWork()
        {
            Debug.Log("=== Testing Wave Start Event ===");

            var waveEvent = new GameEvents.WaveStartEvent(3, 15);
            EventManager.Instance.PublishEvent(waveEvent);

            yield return new WaitForSeconds(0.1f);

            // 验证事件处理
            UnityEngine.Assertions.Assert.IsTrue(waveListener.EventReceived, "Wave start event was not received");
            UnityEngine.Assertions.Assert.AreEqual(3, waveListener.LastWaveNumber);

            Debug.Log("Wave Start Event test completed successfully!");
            yield return null;
        }

        [UnityTest]
        public IEnumerator EventDemo_PlayerLevelUpEvent_ShouldWork()
        {
            Debug.Log("=== Testing Player Level Up Event ===");

            var levelUpEvent = new GameEvents.PlayerLevelUpEvent(5, 1000);
            EventManager.Instance.PublishEvent(levelUpEvent);

            yield return new WaitForSeconds(0.1f);

            // 验证事件处理
            UnityEngine.Assertions.Assert.IsTrue(playerLevelUpListener.EventReceived,
                "Player level up event was not received");
            UnityEngine.Assertions.Assert.AreEqual(5, playerLevelUpListener.LastNewLevel);

            Debug.Log("Player Level Up Event test completed successfully!");
            yield return null;
        }

        [UnityTest]
        public IEnumerator EventDemo_UIRefreshEvent_ShouldWork()
        {
            Debug.Log("=== Testing UI Refresh Event ===");

            var uiEvent = new GameEvents.UIRefreshEvent("MainUI", new { Health = 80, Mana = 60 });
            EventManager.Instance.PublishEvent(uiEvent);

            yield return new WaitForSeconds(0.1f);

            Debug.Log("UI Refresh Event published (no specific listener in this demo)");
            yield return null;
        }

        [UnityTest]
        public IEnumerator EventDemo_FullScenario_ShouldWork()
        {
            Debug.Log("=== Running Full Event Scenario Demo ===");

            // 模拟游戏场景：波次开始 -> 敌人死亡 -> 玩家升级

            // 1. 波次开始
            Debug.Log("1. Starting wave...");
            var waveEvent = new GameEvents.WaveStartEvent(1, 5);
            EventManager.Instance.PublishEvent(waveEvent);
            yield return new WaitForSeconds(0.5f);

            // 2. 敌人死亡
            Debug.Log("2. Enemy dies...");
            var deathEvent = new GameEvents.UnitDeathEvent(1, 2);
            EventManager.Instance.PublishEvent(deathEvent);
            yield return new WaitForSeconds(0.5f);

            // 3. 玩家升级
            Debug.Log("3. Player levels up...");
            var levelUpEvent = new GameEvents.PlayerLevelUpEvent(2, 500);
            EventManager.Instance.PublishEvent(levelUpEvent);
            yield return new WaitForSeconds(0.5f);

            // 4. 波次结束
            Debug.Log("4. Wave ends...");
            var waveEndEvent = new GameEvents.WaveEndEvent(1, true, 120.5f);
            EventManager.Instance.PublishEvent(waveEndEvent);
            yield return new WaitForSeconds(0.5f);

            Debug.Log("=== Full Event Scenario Demo Completed ===");
            yield return null;
        }
    }
}