using Lean.Pool;
using UnityEngine;

namespace Core.Units
{
    public class UnitFactory
    {
        private int _uuid = 0;

        public Unit Spawn(Unit unitPrefab, UnitData data)
        {
            _uuid++;
            data.GUID = _uuid;
            var unit = LeanPool.Spawn(unitPrefab);
            unit.Initialize(data);
            return unit;
        }

        public void Despawn(Unit unit)
        {
            LeanPool.Despawn(unit);
        }
    }
}