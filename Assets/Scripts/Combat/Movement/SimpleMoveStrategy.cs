using Core.Units;
using Utils.Core;

namespace Combat.Movement
{
    public class SimpleMoveStrategy : IMovementStrategy
    {
        public void CalculateMovement(UnitData unit, float deltaTime)
        {
            unit.Position += unit.MoveSpeed * deltaTime * unit.MoveDirection;
            unit.Rotation = MathUtils2D.CalculateRotationAngle(unit.MoveDirection);
        }
    }
}