using Core.Units;
using UnityEngine;

namespace Combat.Movement
{
    public class StraightChaseStrategy : IMovementStrategy
    {
        public void CalculateMovement(UnitData unit, float deltaTime)
        {
            var playerPosition = Vector2.zero; // 假设玩家位置在原点，可以根据实际情况修改
            unit.MoveDirection = (playerPosition - unit.Position).normalized;
            unit.Position += unit.MoveSpeed * deltaTime * unit.MoveDirection;
        }
    }
}