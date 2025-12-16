using System;
using System.Collections.Generic;
using System.Linq;
using Core.Events;
using Core.Units;
using UnityEngine;

namespace Combat.Systems
{
    /// <summary>
    ///     单位重叠检测系统
    ///     纯逻辑计算，不依赖MonoBehaviour
    /// </summary>
    public class UnitOverlapSystem
    {
        private readonly Dictionary<int, UnitData> _units;

        public UnitOverlapSystem(Dictionary<int, UnitData> units)
        {
            _units = units;
        }

        /// <summary>
        ///     检测两个单位是否重叠
        /// </summary>
        public bool IsOverlapping(UnitData unitA, UnitData unitB)
        {
            if (!unitA.IsActive || !unitB.IsActive)
                return false;

            if (unitA.GUID == unitB.GUID)
                return false;

            var areaA = unitA.GetCollisionArea();
            var areaB = unitB.GetCollisionArea();

            return areaA.Intersects(areaB);
        }

        /// <summary>
        ///     获取与指定单位重叠的所有单位
        /// </summary>
        public List<UnitData> GetOverlappingUnits(UnitData targetUnit)
        {
            var overlappingUnits = new List<UnitData>();

            foreach (var unit in _units.Values)
                if (IsOverlapping(targetUnit, unit))
                    overlappingUnits.Add(unit);

            return overlappingUnits;
        }

        /// <summary>
        ///     获取与指定单位重叠的特定类型单位
        /// </summary>
        public List<UnitData> GetOverlappingUnits(UnitData targetUnit, Func<UnitData, bool> filter)
        {
            return GetOverlappingUnits(targetUnit).Where(filter).ToList();
        }

        /// <summary>
        ///     检测指定位置是否与任何单位重叠
        /// </summary>
        public bool IsPositionOccupied(Vector2 position, UnitCollisionData collisionData, int excludeUnitId = -1)
        {
            var tempUnit = new UnitData(position, 0, collisionData) { GUID = -1 };

            foreach (var unit in _units.Values)
            {
                if (unit.GUID == excludeUnitId)
                    continue;

                if (IsOverlapping(tempUnit, unit))
                    return true;
            }

            return false;
        }

        /// <summary>
        ///     检测所有单位重叠对
        /// </summary>
        public List<(UnitData unitA, UnitData unitB)> GetAllOverlappingPairs()
        {
            var overlappingPairs = new List<(UnitData, UnitData)>();
            var unitList = _units.Values.Where(u => u.IsActive).ToList();

            for (var i = 0; i < unitList.Count; i++)
            for (var j = i + 1; j < unitList.Count; j++)
                if (IsOverlapping(unitList[i], unitList[j]))
                    overlappingPairs.Add((unitList[i], unitList[j]));

            return overlappingPairs;
        }

        /// <summary>
        ///     空间分区优化版本（用于大量单位时）
        /// </summary>
        public List<UnitData> GetOverlappingUnitsOptimized(UnitData targetUnit, float gridSize = 10f)
        {
            // 简单的网格空间分区
            var targetArea = targetUnit.GetCollisionArea();
            var bounds = targetArea.GetBounds();

            var candidateUnits = new List<UnitData>();

            // 只检测附近网格内的单位
            foreach (var unit in _units.Values)
            {
                if (!unit.IsActive || unit.GUID == targetUnit.GUID)
                    continue;

                // 粗略距离检测
                var distance = Vector2.Distance(unit.Position, targetUnit.Position);
                var maxCheckDistance = GetMaxCollisionRadius(unit) + GetMaxCollisionRadius(targetUnit);

                if (distance <= maxCheckDistance) candidateUnits.Add(unit);
            }

            // 精确碰撞检测
            return candidateUnits.Where(unit => IsOverlapping(targetUnit, unit)).ToList();
        }

        private float GetMaxCollisionRadius(UnitData unit)
        {
            return unit.CollisionData.AreaType switch
            {
                CollisionAreaType.Circle => unit.CollisionData.Radius,
                CollisionAreaType.Rectangle => Mathf.Max(unit.CollisionData.Size.x, unit.CollisionData.Size.y) * 0.5f,
                _ => 1f
            };
        }

        public void TickCheckOverlap(float deltaTime)
        {
            var overlappingPairs = GetAllOverlappingPairs();
            foreach (var (unitA, unitB) in overlappingPairs)
                // Debug.Log($"Units {unitA.GUID} and {unitB.GUID} are overlapping.");
                EventManager.Instance.Publish(new GameEvents.OverlapEvent(unitA.GUID, unitB.GUID));
        }
    }
}