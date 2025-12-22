using System.Collections.Generic;
using Combat.Systems;
using Core.Units;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Unit
{
    [TestFixture]
    public class OverlapSystemTest
    {
        private Dictionary<int, UnitData> _allUnits;
        private UnitOverlapSystem _unitOverlapSystem;

        [SetUp]
        public void Setup()
        {
            _allUnits = new Dictionary<int, UnitData>();
            _unitOverlapSystem = new UnitOverlapSystem(_allUnits);
            Debug.Log("TestUnitOverlap started.");
        }

        [TearDown]
        public void Teardown()
        {
            _allUnits.Clear();
            _unitOverlapSystem = null;
            Debug.Log("TestUnitOverlap teardown.");
        }

        [Test]
        public void TestUnitOverlapCircle()
        {
            Debug.Log("TestUnitOverlapCircle started.");
            _allUnits.Clear();
            var unit = new UnitData(new Vector2(0, 0))
            {
                RuntimeId = 1,
                IsActive = true,
                CollisionData = new UnitCollisionData
                {
                    Radius = 1.0f,
                    AreaType = CollisionAreaType.Circle
                }
            };

            var unit2 = new UnitData(new Vector2(0, 0))
            {
                RuntimeId = 2,
                IsActive = true,
                CollisionData = new UnitCollisionData
                {
                    Radius = 1.0f,
                    AreaType = CollisionAreaType.Circle
                }
            };

            _allUnits[unit.RuntimeId] = unit;
            _allUnits[unit2.RuntimeId] = unit2;
            var isOverlapping = _unitOverlapSystem.IsOverlapping(unit, unit2);

            Assert.IsTrue(isOverlapping, "Units should be overlapping.");
        }

        [Test]
        public void TestUnitOverlapRectangle()
        {
            Debug.Log("TestUnitOverlapRectangle started.");
            _allUnits.Clear();
            var unit = new UnitData(new Vector2(0, 0))
            {
                RuntimeId = 1,
                IsActive = true,
                CollisionData = new UnitCollisionData
                {
                    AreaType = CollisionAreaType.Rectangle,
                    Size = new Vector2(2.0f, 2.0f)
                }
            };

            var unit2 = new UnitData(new Vector2(0, 0))
            {
                RuntimeId = 2,
                IsActive = true,
                CollisionData = new UnitCollisionData
                {
                    AreaType = CollisionAreaType.Rectangle,
                    Size = new Vector2(2.0f, 2.0f)
                }
            };

            _allUnits[unit.RuntimeId] = unit;
            _allUnits[unit2.RuntimeId] = unit2;
            var isOverlapping = _unitOverlapSystem.IsOverlapping(unit, unit2);

            Assert.IsTrue(isOverlapping, "Units should be overlapping.");
        }
    }
}