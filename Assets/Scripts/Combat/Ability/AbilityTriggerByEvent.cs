using Combat.Ability.Data;
using Core.Events;
using Core.Units;

namespace Combat.Ability
{
    public class AbilityTriggerByEvent : BaseAbility
    {
        private AbilityTriggerByEventData AbilityDataOverride { get; set; }
        private TargetSet TempTargetSet { get; set; }

        public AbilityTriggerByEvent(AbilityTriggerByEventData data) : base(data)
        {
            AbilityDataOverride = data;
            TempTargetSet = new TargetSet();
        }

        public override void ApplyAbility()
        {
            base.ApplyAbility();
            foreach (var triggerEventType in AbilityDataOverride.EventTypes)
            {
                // 根据不同的事件类型注册相应的事件监听器
                switch (triggerEventType)
                {
                    case TriggerEventType.OnCollideOtherUnit:
                        EventManager.Instance.Subscribe<GameEvents.OverlapEvent>(OnUnitCollide);
                        break;
                    // 可以添加更多的事件类型处理
                }
            }
        }

        public override void RemoveAbility()
        {
            base.RemoveAbility();
            foreach (var triggerEventType in AbilityDataOverride.EventTypes)
            {
                // 注销相应的事件监听器
                switch (triggerEventType)
                {
                    case TriggerEventType.OnCollideOtherUnit:
                        EventManager.Instance.Unsubscribe<GameEvents.OverlapEvent>(OnUnitCollide);
                        break;
                    // 可以添加更多的事件类型处理
                }
            }

            AbilityDataOverride = null;
        }

        /// <summary>
        /// 这里处理单位碰撞事件
        /// TODO: 应该一次传入所有碰撞对象
        /// </summary>
        /// <param name="obj"></param>
        private void OnUnitCollide(GameEvents.OverlapEvent obj)
        {
            if (obj.UnitAGuid != UnitId) return;
            // 触发能力效果
            TempTargetSet.TargetUnits.Clear();
            TempTargetSet.TargetUnits.Add(obj.UnitBGuid);
            TryCastAbility();
        }

        /// <summary>
        /// 重写目标查找逻辑，事件触发时已指定目标
        /// 需要再筛选一遍
        /// </summary>
        public override void FindTargets()
        {
            switch (AbilityData.FindTargetType)
            {
                case FindTargetType.Specific:
                    var targetId = (int)AbilityData.ExtraParams["TargetUnitId"];
                    Targets.TargetUnits.Clear();
                    if (TempTargetSet.TargetUnits.Contains(targetId))
                    {
                        Targets.TargetUnits.Add(targetId);
                    }

                    break;
                case FindTargetType.Enemy:
                    Targets.TargetUnits.Clear();
                    FindTargetUnit(GroupType.Enemy);
                    break;
                case FindTargetType.Ally:
                    Targets.TargetUnits.Clear();
                    FindTargetUnit(GroupType.Hero);
                    break;
                case FindTargetType.Self:
                default:
                    break;
            }
        }

        protected override void FindTargetUnit(GroupType targetUnitGroup)
        {
            foreach (var targetUnitId in TempTargetSet.TargetUnits)
            {
                if (UnitManager.Instance.TryGetAvailableUnit(targetUnitId, out var targetUnit) &&
                    targetUnit.Group == targetUnitGroup)
                {
                    Targets.TargetUnits.Add(targetUnitId);
                    return;
                }
            }
        }
    }
}