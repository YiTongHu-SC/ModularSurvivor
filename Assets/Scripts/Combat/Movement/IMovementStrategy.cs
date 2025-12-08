using Core.Units;
using UnityEngine;

namespace Combat.Movement
{
    public struct MovementContext
    {
        public Vector2 currentPosition;
        public Vector2 targetPosition;
        public float speed;
        public LayerMask obstacles;
        public Bounds movementBounds;
    }

    public interface IMovementStrategy
    {
        public void CalculateMovement(UnitData unitData, float deltaTime);
    }
}