using System.Collections.Generic;
using Core.Events;
using Core.Units;
using UnityEngine;

namespace Combat.Systems
{
    public class MovementSystem
    {
        private static Dictionary<int, UnitData> MovingUnits => UnitManager.Instance.Units;
        private Vector2 playerPosition;

        public void UpdateMovement(float deltaTime)
        {
            foreach (var unit in MovingUnits.Values)
            {
                var direction = (playerPosition - unit.Position).normalized;
                unit.Position += unit.MoveSpeed * deltaTime * direction;
                // // 通知View层更新
                // EventBus.Publish(new UnitMovedEvent(unit.id, unit.position));
                EventManager.Instance.PublishEvent(new GameEvents.UnitMovementEvent(unit));
            }
        }
    }
}