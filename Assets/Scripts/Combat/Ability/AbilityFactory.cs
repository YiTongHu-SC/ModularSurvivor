using System;
using System.Collections.Generic;
using Combat.Ability.Data;

namespace Combat.Ability
{
    public static class AbilityFactory
    {
        // key: BuffId，value: 创建该 Buff 的委托
        private static readonly Dictionary<TriggerType, Func<AbilityData, BaseAbility>> _creators
            = new()
            {
                {
                    TriggerType.Active,
                    (data) => new AbilityPassive(data)
                },
                {
                    TriggerType.Passive,
                    (data) => new AbilityPassive(data)
                },
                {
                    TriggerType.Interval,
                    (data) => new AbilityPassive(data)
                },
                {
                    TriggerType.ByEvent,
                    (data) => new AbilityTriggerByEvent(data as AbilityTriggerByEventData)
                },
                {
                    TriggerType.Once,
                    (data) => new AbilityTriggerOnce(data)
                },
            };

        public static BaseAbility CreateAbility(AbilityData abilityData)
        {
            if (!_creators.TryGetValue(abilityData.TriggerType, out var creator))
            {
                throw new ArgumentException($"未找到对应的 Ability 创建器: {abilityData.TriggerType} for {abilityData}");
            }

            return creator(abilityData);
        }
    }
}