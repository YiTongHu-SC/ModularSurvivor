using Combat.Ability.Data;
using Core.Input;
using UnityEngine;

namespace Combat.Ability
{
    public class AbilityPlayerInput : BaseAbility
    {
        private Vector2 _moveDirection;
        private float _deadZone;

        public AbilityPlayerInput(PlayerInputData abilityData, int id) : base(abilityData, id)
        {
            _deadZone = abilityData.DeadZone;
        }

        public override void ApplyAbility()
        {
        }

        public override void RemoveAbility()
        {
        }

        public override void UpdateAbility(float deltaTime)
        {
            _moveDirection = InputManager.Instance.GetMoveDirection();
            if (_moveDirection.magnitude < _deadZone)
            {
                _moveDirection = Vector2.zero;
            }
            else
            {
                _moveDirection.Normalize();
            }

            UnitData.MoveDirection = _moveDirection;
        }

        public override void PerformAbility()
        {
        }
    }
}