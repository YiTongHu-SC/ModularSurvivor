using Core.Units;
using UnityEngine;
using Utils.Core;

namespace Combat.Movement
{
    public class SimpleMoveStrategy : IMovementStrategy
    {
        public void CalculateMovement(UnitData unit, float deltaTime, MovementContext context = default)
        {
            unit.Position += unit.MoveSpeed * deltaTime * unit.MoveDirection;
            if (unit.MoveDirection != Vector2.zero)
            {
                unit.Rotation = MathUtils2D.CalculateRotationAngle(unit.MoveDirection);
            }
        }
    }
}