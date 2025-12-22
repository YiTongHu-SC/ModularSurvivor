using Combat.Ability.Data;
using Combat.Systems;
using Core.Events;
using Core.Units;
using UnityEngine;

namespace Combat.Ability
{
    public class AbilityHitOnceOnCollision : BaseAbility, IEventListener<GameEvents.OverlapEvent>
    {
        public HitOnceOnCollisionData AbilityAbilityData { get; private set; }
        private bool _hasHit;
        private float _elapsedTime;

        public AbilityHitOnceOnCollision(HitOnceOnCollisionData abilityData, int id) : base(abilityData, id)
        {
            AbilityAbilityData = abilityData;
        }

        public override void ApplyAbility()
        {
            _hasHit = false;
            EventManager.Instance.Subscribe<GameEvents.OverlapEvent>(this);
        }

        public override void RemoveAbility()
        {
            EventManager.Instance.Unsubscribe<GameEvents.OverlapEvent>(this);
        }

        public override void UpdateAbility(float deltaTime)
        {
            if (!IsActive) return;
            if (!_hasHit) return;
            _elapsedTime += deltaTime;
            if (_elapsedTime >= AbilityAbilityData.HitCooldown)
            {
                // 冷却时间到，重置状态以允许再次命中
                _elapsedTime = 0f;
                _hasHit = false;
            }
        }

        public override void PerformAbility()
        {
        }

        public void OnEventReceived(GameEvents.OverlapEvent eventData)
        {
            if (!IsActive) return;
            if (_hasHit) return;
            if (eventData.UnitAGuid != UnitId) return;

            var otherUnitId = eventData.UnitBGuid;
            if (!UnitManager.Instance.Units.TryGetValue(otherUnitId, out var otherUnit))
            {
                return; // 目标单位不存在
            }

            if (!otherUnit.IsActive) return; // 目标单位已经无效
            if (otherUnit.Group == UnitData.Group) return; // 忽略友军
            // 这里可以添加命中逻辑，例如造成伤害或应用效果
            CombatManager.Instance.DamageSystem.TryApplyDamage(
                otherUnit, AbilityAbilityData.DamageAmount, UnitData);

            Debug.Log($"{UnitId} apply {AbilityAbilityData.DamageAmount} damage to {otherUnitId}");
            _hasHit = true; // 只命中一次后失效
        }
    }
}