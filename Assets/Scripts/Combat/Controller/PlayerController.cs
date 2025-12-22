using Core.Input;
using Core.Units;
using UnityEngine;

namespace Combat.Controller
{
    public class PlayerController : MonoBehaviour
    {
        public PlayerControllerConfig Config;
        public int TargetId;

        public void SetTarget(int targetId)
        {
            TargetId = targetId;
        }

        private void Update()
        {
            if (!UnitManager.Instance.TryGetAvailableUnit(TargetId, out var unitData))
            {
                Debug.LogError($"Target ID {TargetId} not found.");
                return;
            }

            var moveDirection = InputManager.Instance.GetMoveDirection();
            if (moveDirection.magnitude < Config.DeadZone)
            {
                Debug.Log($"Player is moving in direction: {moveDirection}");
            }
            else
            {
                moveDirection.Normalize();
            }

            unitData.MoveDirection = moveDirection;
        }
    }
}