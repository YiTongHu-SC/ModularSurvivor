using System.Collections.Generic;
using Combat.Movement;
using Core.Events;
using Core.Units;
using UnityEngine;

namespace Combat.Systems
{
    public class MovementSystem
    {
        private static Dictionary<int, UnitData> MovingUnits => UnitManager.Instance.Units;
        private readonly Dictionary<string, IMovementStrategy> _movementStrategies = new();
        private Vector2 playerPosition;

        public void Initialize()
        {
            playerPosition = Vector2.zero;
            _movementStrategies.Add("StraightChase", new StraightChaseStrategy());
        }

        /// <summary>
        /// 更新单位位置
        /// </summary>
        /// <param name="deltaTime"></param>
        public void UpdateMovement(float deltaTime)
        {
            foreach (var unit in MovingUnits.Values)
            {
                // // 通知View层更新
                if (_movementStrategies.ContainsKey(unit.MovementStrategy))
                {
                    _movementStrategies[unit.MovementStrategy].CalculateMovement(unit, deltaTime);
                    EventManager.Instance.PublishEvent(new GameEvents.UnitMovementEvent(unit));
                }
            }
        }
    }
}