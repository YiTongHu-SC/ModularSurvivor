using Core.Units;
using Utils.Core;

namespace Combat.Movement
{
    public class StraightChaseStrategy : IMovementStrategy
    {
        public void CalculateMovement(UnitData unit, float deltaTime, MovementContext context = default)
        {
            unit.MoveDirection = (context.targetPosition - unit.Position).normalized;
            unit.Position += unit.MoveSpeed * deltaTime * unit.MoveDirection;
            // 计算旋转角度，确保单位面向移动方向
            unit.Rotation = MathUtils2D.CalculateRotationAngle(unit.MoveDirection);
        }
    }
}