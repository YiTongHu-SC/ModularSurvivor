using System.Collections;
using Combat.Actors;
using Combat.Buff;
using Combat.Systems;
using Core.Events;
using Core.Units;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Combat
{
    public class BuffSystemTests
    {
        private CombatManager _combatManager;
        private UnitManager _unitManager;
        private EventManager _eventManager;
        private const float DeltaTime = 0.01f;
        private const float Tolerance = 0.001f;

        [SetUp]
        public void Setup()
        {
            var eventManagerGo = new GameObject("EventManager");
            _eventManager = eventManagerGo.AddComponent<EventManager>();
            _eventManager.Initialize();
            // 初始化测试所需的资源和状态
            var unitManagerGo = new GameObject("UnitManager");
            _unitManager = unitManagerGo.AddComponent<UnitManager>();
            _unitManager.Initialize();
            var combatManager = new GameObject("CombatManager");
            _combatManager = combatManager.AddComponent<CombatManager>();
            _combatManager.Initialize();
        }

        [TearDown]
        public void Teardown()
        {
            // 清理测试所使用的资源和状态
            Object.DestroyImmediate(_combatManager.gameObject);
            Object.DestroyImmediate(_unitManager.gameObject);
            Object.DestroyImmediate(_eventManager.gameObject);
        }

        [Test]
        public void BuffSystemTestsSimplePasses()
        {
            // Use the Assert class to test conditions.
            Assert.IsNotNull(_combatManager.BuffSystem);
        }

        // A UnityTest behaves like a coroutine in PlayMode
        // and allows you to yield null to skip a frame in EditMode
        [UnityTest]
        public IEnumerator BuffSystemTestsWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // yield to skip a frame
            yield return null;
            Assert.IsNotNull(_combatManager.MovementSystem);
        }

        [UnityTest]
        public IEnumerator TestApplyBuffToUnit()
        {
            // 创建一个测试单位
            yield return null;
            Assert.IsNotNull(_combatManager.BuffSystem);
            var testUnit = new UnitData(new Vector2(0, 0), 0)
            {
                RuntimeId = 1,
            };
            testUnit.MoveSpeed = 10f;
            var actorGo = CreateGameObject();
            var actor = _combatManager.ActorFactory.Spawn(actorGo, testUnit);
            var buffData = new BuffData(0,
                "SpeedBoost",
                BuffType.SpeedBoost,
                5f,
                2.0f);
            // 应用Buff
            var result = _combatManager.BuffSystem.ApplyBuff(BuffType.SpeedBoost, buffData, testUnit.RuntimeId);
            Assert.IsTrue(result, "Buff apply should succeed.");
            // 验证Buff效果
            Assert.AreEqual(20f, testUnit.MoveSpeed, Tolerance, "Unit move speed should be boosted.");
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestBuffExpiration()
        {
            yield return null;
            Assert.IsNotNull(_combatManager.BuffSystem);
            var testUnit = new UnitData(new Vector2(0, 0), 0)
            {
                RuntimeId = 2,
            };
            testUnit.MoveSpeed = 10f;
            var actorGo = CreateGameObject();
            var actor = _combatManager.ActorFactory.Spawn(actorGo, testUnit);
            var delayDuration = 1f;
            var buffData = new BuffData(0,
                "DelayDeath",
                BuffType.DelayDeath,
                delayDuration); // 1秒后过期,过期时死亡
            // 应用Buff
            var result = _combatManager.BuffSystem.ApplyBuff(buffData.BuffType, buffData, testUnit.RuntimeId);
            Assert.IsTrue(result, "Buff apply should succeed.");
            // 等待Buff过期
            float elapsed = 0f;
            while (elapsed < delayDuration)
            {
                _combatManager.BuffSystem.UpdateBuffs(DeltaTime);
                elapsed += DeltaTime;
                yield return null;
            }

            // 最后一次更新以确保Buff过期
            _combatManager.BuffSystem.UpdateBuffs(DeltaTime);
            // 验证单位已死亡
            Assert.IsFalse(testUnit.IsActive, "Unit should be dead after DelayDeath buff expires");
        }

        private Actor CreateGameObject()
        {
            // 创建测试用的简单预制体
            // 创建测试用的简单预制体
            var testUnitPrefab = new GameObject("TestUnit");
            var actor = testUnitPrefab.AddComponent<Actor>(); // 假设有Unit组件

            var initialPosition = new Vector2(0, 0);
            var unitData = new UnitData(initialPosition, 0);
            unitData.MoveSpeed = 5f;
            return actor;
        }
    }
}