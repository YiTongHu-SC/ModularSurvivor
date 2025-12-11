using Core.Units;

namespace Combat.Movement
{
    public interface IMovementStrategy
    {
        public void CalculateMovement(UnitData unit, float deltaTime, MovementContext context = default);
    }
}