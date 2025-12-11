using Combat.Ability.Data;
using Combat.Systems;
using Combat.Views.UnitViewData;
using Core.Events;
using Core.Timer;
using Core.Units;
using UnityEngine;

namespace Combat.Ability
{
    public class AbilityLaserStrike : BaseAbility
    {
        private int _targetUnitId = -1;
        private readonly LaserStrikeData _abilityData;
        private readonly UnitData _tempAbilityUnitData;
        private bool _hasHit;
        private bool _isCoolingDown;
        private OneStrikeViewData _oneStrikeViewData;
        private GameEvents.UpdatePreferenceEvent _presentationEvent;

        public AbilityLaserStrike(LaserStrikeData abilityData, int id) : base(abilityData, id)
        {
            _abilityData = abilityData;
            _tempAbilityUnitData = new UnitData(Vector2.zero, 0f, collisionData: abilityData.collisionData);
        }

        public override void ApplyAbility()
        {
            _targetUnitId = -1;
            _hasHit = false;
            _isCoolingDown = false;
            _tempAbilityUnitData.IsActive = true;
            _oneStrikeViewData = new OneStrikeViewData
            {
                UnitId = UnitId,
                TargetId = _targetUnitId,
                PresentationId = _abilityData.PresentationId,
                Delay = 0,
                Duration = _abilityData.HitDuration,
            };
            _presentationEvent = new GameEvents.UpdatePreferenceEvent(_abilityData.PresentationId, _oneStrikeViewData);
        }

        public override void RemoveAbility()
        {
        }

        public override void UpdateAbility(float deltaTime)
        {
            if (_hasHit || _isCoolingDown) return;
            _tempAbilityUnitData.Position = UnitData.Position;
            FindTargetUnit();
            PerformAbility();
        }

        private void FindTargetUnit()
        {
            var overlappingUnits = UnitManager.Instance.OverlapSystem.GetOverlappingUnits(_tempAbilityUnitData, Filter);

            bool Filter(UnitData targetUnit)
            {
                return (targetUnit.Group != UnitData.Group) && (targetUnit.Position - UnitData.Position).magnitude <
                    _abilityData.collisionData.Radius;
            }

            if (overlappingUnits.Count == 0)
            {
                _targetUnitId = -1;
            }
            else
            {
                // 选择距离最近的单位作为目标
                UnitData closestUnit = null;
                float closestDistanceSqr = float.MaxValue;
                foreach (var unit in overlappingUnits)
                {
                    float distanceSqr = (unit.Position - UnitData.Position).sqrMagnitude;
                    if (distanceSqr < closestDistanceSqr)
                    {
                        closestDistanceSqr = distanceSqr;
                        closestUnit = unit;
                    }
                }

                if (closestUnit != null) _targetUnitId = closestUnit.GUID;
            }
        }

        public override void PerformAbility()
        {
            if (_targetUnitId == -1) return;
            _hasHit = true;
            _isCoolingDown = true;
            _oneStrikeViewData.Delay = 0;
            _oneStrikeViewData.Duration = _abilityData.HitDuration;
            _oneStrikeViewData.TargetId = _targetUnitId;
            EventManager.Instance.PublishEvent(_presentationEvent);
            TimeManager.Instance.TimeSystem.CreateTimer(_abilityData.HitDuration, HitTarget);
            TimeManager.Instance.TimeSystem.CreateTimer(_abilityData.HitCooldown, ResetCooldown);
        }

        private void HitTarget()
        {
            if (UnitManager.Instance.Units.TryGetValue(_targetUnitId, out var targetUnit))
            {
                CombatManager.Instance.DamageSystem.TryApplyDamage(targetUnit, _abilityData.DamageAmount, UnitData);
            }

            ResetHit();
        }

        private void ResetHit()
        {
            _hasHit = false;
        }

        private void ResetCooldown()
        {
            _isCoolingDown = false;
        }
    }
}