using System.Collections;
using Combat.Actors;
using Combat.Systems;
using Core.Events;
using Core.Units;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Combat
{
    public class MoveSystemTests
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
            // 确保 UnitManager 的 Awake 方法被调用以初始化单例
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
        public void TestMoveSystemSimplePasses()
        {
            // Use the Assert class to test conditions.
            Assert.IsNotNull(_combatManager.MovementSystem);
        }

        // A UnityTest behaves like a coroutine in PlayMode
        // and allows you to yield null to skip a frame in EditMode
        [UnityTest]
        public IEnumerator TestMoveSystemWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // yield to skip a frame
            yield return null;
            Assert.IsNotNull(_combatManager.MovementSystem);
        }

        [UnityTest]
        public IEnumerator TestUnitMovement()
        {
            yield return null;
            Assert.IsNotNull(_combatManager.MovementSystem);
            yield return null;
            var initialPosition = new Vector2(0, 0);
            var unitData = new UnitData(initialPosition, 0);
            unitData.MoveSpeed = 5f;
            unitData.MoveDirection = Vector2.right; // 向右移动
            var actorGo = CreateGameObject();
            var actor = _combatManager.ActorFactory.Spawn(actorGo, unitData);
            yield return null;
            _combatManager.MovementSystem.Tick(DeltaTime);
            var expectedPosition = initialPosition + unitData.MoveSpeed * DeltaTime * unitData.MoveDirection;
            Assert.AreEqual(expectedPosition.x, unitData.Position.x, Tolerance);
            Assert.AreEqual(expectedPosition.y, unitData.Position.y, Tolerance);
        }

        private Actor CreateGameObject()
        {
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