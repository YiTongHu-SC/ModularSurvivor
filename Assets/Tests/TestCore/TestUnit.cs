using System;
using Core.Units;
using UnityEngine;

namespace Tests.TestCore
{
    public class TestUnit : MonoBehaviour
    {
        public Unit UnitPrefab;

        private void Start()
        {
            PerformTest();
        }

        private void PerformTest()
        {
            var unitData = new UnitData(new Vector2(3, 4), 90f);
            Debug.Assert(unitData.Position == new Vector2(3, 4), "UnitData position is incorrect.");
            Debug.Assert(Math.Abs(unitData.Rotation - 90f) < 0.01f, "UnitData rotation is incorrect.");
            var unit = UnitManager.Instance.Factory.Spawn(UnitPrefab, unitData);
            Debug.Assert(unit.UnitData.Position == new Vector2(3, 4), "Unit position is incorrect.");
            Debug.Assert(Math.Abs(unit.UnitData.Rotation - 90f) < 0.01f, "Unit rotation is incorrect.");
        }
    }
}