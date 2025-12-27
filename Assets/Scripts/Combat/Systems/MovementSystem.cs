using System;
using System.Collections.Generic;
using Combat.Movement;
using Core.Events;
using Core.GameInterface;
using Core.Units;

namespace Combat.Systems
{
    public class MovementSystem : ISystem
    {
        private readonly Dictionary<string, IMovementStrategy> _movementStrategies = new();
        private static Dictionary<int, UnitData> MovingUnits => UnitManager.Instance.Units;

        public void Initialize()
        {
            _movementStrategies.Add("SimpleMove", new SimpleMoveStrategy());
            _movementStrategies.Add("StraightChase", new StraightChaseStrategy());
        }

        public void Reset()
        {
            _movementStrategies.Clear();
        }

        /// <summary>
        /// 更新单位位置
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Tick(float deltaTime)
        {
            foreach (var unit in MovingUnits.Values)
            {
                // // 通知View层更新
                if (_movementStrategies.TryGetValue(unit.MovementStrategy, out var strategy))
                {
                    strategy.CalculateMovement(unit, deltaTime, unit.GetMovementContext());
                    EventManager.Instance.Publish(new GameEvents.UnitMovementEvent(unit));
                }
            }
        }
    }
}