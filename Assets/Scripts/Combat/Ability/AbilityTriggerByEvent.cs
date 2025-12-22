using Combat.Ability.Data;
using Core.Events;

namespace Combat.Ability
{
    public class AbilityTriggerByEvent : BaseAbility
    {
        private AbilityTriggerByEventData AbilityDataOverride { get; set; }

        public AbilityTriggerByEvent(AbilityTriggerByEventData data) : base(data)
        {
            AbilityDataOverride = data;
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

        private void OnUnitCollide(GameEvents.OverlapEvent obj)
        {
            if (obj.UnitAGuid != UnitId) return;
            // 触发能力效果
            Context.Targets.TaregetUnits.Clear();
            Context.Targets.TaregetUnits.Add(obj.UnitBGuid);
            TryCastAbility();
        }
    }
}