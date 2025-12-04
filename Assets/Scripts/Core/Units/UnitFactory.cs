using Lean.Pool;
using UnityEngine;

namespace Core.Units
{
    public class UnitFactory
    {
        public Unit Spawn(Unit UnitPrefab, UnitData data)
        {
            var unit = LeanPool.Spawn(UnitPrefab);
            unit.Initialize(data);
            return unit;
        }

        public void Despawn(Unit unit)
        {
            LeanPool.Despawn(unit);
        }
    }
}