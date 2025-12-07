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

        public void Initialize()
        {
            playerPosition = Vector2.zero;
        }

        /// <summary>
        /// 更新单位位置
        /// </summary>
        /// <param name="deltaTime"></param>
        public void UpdateMovement(float deltaTime)
        {
            foreach (var unit in MovingUnits.Values)
            {
                unit.MoveDirection = (playerPosition - unit.Position).normalized;
                unit.Position += unit.MoveSpeed * deltaTime * unit.MoveDirection;
                // // 通知View层更新
                EventManager.Instance.PublishEvent(new GameEvents.UnitMovementEvent(unit));
            }
        }
    }
}