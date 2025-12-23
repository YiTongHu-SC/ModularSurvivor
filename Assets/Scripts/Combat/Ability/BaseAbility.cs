using Combat.Ability.Data;
using Combat.Effect;
using Combat.Systems;
using Core.Events;
using Core.Units;
using UnityEngine;

namespace Combat.Ability
{
    public abstract class BaseAbility
    {
        public int UnitId { get; private set; }
        public AbilityData AbilityData { get; private set; }
        public bool IsActive { get; set; } = false;
        public bool IsExpired => !IsActive;
        protected AbilityContext Context { get; private set; }
        protected TargetSet Targets { get; private set; }
        protected bool IsOnCooldown { get; set; } = false;
        private float CooldownTimer { get; set; } = 0;
        private UnitData TempAbilityUnitData;
        private GroupType TargetGroup { get; set; }

        protected BaseAbility(AbilityData data)
        {
            IsActive = true;
            UnitId = data.SourceId;
            AbilityData = data;
            Targets = new TargetSet();
            Context = new AbilityContext(UnitId, Targets);
        }

        /// <summary>
        /// 应用能力效果，新增能力时调用一次
        /// </summary>
        public virtual void ApplyAbility()
        {
            IsOnCooldown = false;
            CooldownTimer = 0;
            TempAbilityUnitData = new UnitData(Vector2.zero)
            {
                IsActive = false,
                RuntimeId = CombatManager.Instance.GlobalAllocator.Next(),
            };
        }

        /// <summary>
        /// 释放能力效果，满足条件时调用
        /// </summary>
        protected virtual void TryCastAbility()
        {
            if (!IsActive || IsOnCooldown) return;
            if (!UnitManager.Instance.CheckUnitAvailability(UnitId))
            {
                IsActive = false;
                return;
            }

            FindTargets();
            var effect = EffectFactory.CreateEffectNode(AbilityData.EffectSpec);
            effect.SetContext(Context);
            CombatManager.Instance.EffectSystem.CastEffect(effect);
            IsOnCooldown = true;
            CooldownTimer = 0;
        }

        /// <summary>
        /// 移除能力效果，能力结束时调用一次
        /// </summary>
        public virtual void RemoveAbility()
        {
            IsActive = false;
            Targets = null;
            Context = null;
            UnitId = 0;
        }

        /// <summary>
        /// 更新能力状态，每帧调用
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        public virtual void TickAbility(float deltaTime)
        {
            // 检查单位是否仍然存在,否则移除能力
            if (!UnitManager.Instance.CheckUnitAvailability(UnitId))
            {
                IsActive = false;
            }

            if (!IsActive) return;
            // 更新冷却时间
            if (IsOnCooldown)
            {
                CooldownTimer += deltaTime;
                if (CooldownTimer >= AbilityData.Cooldown)
                {
                    IsOnCooldown = false;
                    CooldownTimer = 0;
                }
            }
        }

        public virtual void FindTargets()
        {
            switch (AbilityData.FindTargetType)
            {
                case FindTargetType.Specific:
                    Targets.TargetUnits.Clear();
                    Targets.TargetUnits.Add((int)AbilityData.ExtraParams["TargetUnitId"]);
                    break;
                case FindTargetType.Self:
                    Targets.TargetUnits.Clear();
                    Targets.TargetUnits.Add(UnitId);
                    break;
                case FindTargetType.Enemy:
                    Targets.TargetUnits.Clear();
                    FindTargetUnit(GroupType.Enemy);
                    break;
                case FindTargetType.Ally:
                    Targets.TargetUnits.Clear();
                    FindTargetUnit(GroupType.Ally);
                    break;
                default:
                    break;
            }
        }

        private void RefreshTempAbilityUnitData()
        {
            var unitData = UnitManager.Instance.Units[UnitId];
            TempAbilityUnitData.IsActive = true;
            TempAbilityUnitData.Group = unitData.Group;
            TempAbilityUnitData.Position = unitData.Position;
            TempAbilityUnitData.CollisionData = (UnitCollisionData)AbilityData.ExtraParams["CollisionData"];
        }

        protected virtual void FindTargetUnit(GroupType targetUnitGroup)
        {
            TargetGroup = targetUnitGroup;
            var unitData = UnitManager.Instance.Units[UnitId];
            RefreshTempAbilityUnitData();
            var overlappingUnits =
                UnitManager.Instance.OverlapSystem.GetOverlappingUnits(TempAbilityUnitData, FindTargetFilter);

            if (overlappingUnits.Count <= 0) return;
            // 选择距离最近的单位作为目标
            UnitData closestUnit = null;
            float closestDistanceSqr = float.MaxValue;
            foreach (var unit in overlappingUnits)
            {
                float distanceSqr = (unit.Position - unitData.Position).sqrMagnitude;
                if (distanceSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqr;
                    closestUnit = unit;
                }
            }

            if (closestUnit != null)
            {
                Targets.TargetUnits.Add(closestUnit.RuntimeId);
            }

            return;
        }

        protected virtual bool FindTargetFilter(UnitData targetUnit)
        {
            return (targetUnit.Group == TargetGroup) && (targetUnit.Position - TempAbilityUnitData.Position).magnitude <
                TempAbilityUnitData.CollisionData.Radius;
        }
    }
}