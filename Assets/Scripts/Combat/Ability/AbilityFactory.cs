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
                    TriggerType.HitOnceOnCollision,
                    (data) => new AbilityPassive(data)
                },
            };

        public static BaseAbility CreateAbility(TriggerType triggerType, AbilityData abilityData)
        {
            if (!_creators.TryGetValue(triggerType, out var creator))
            {
                throw new ArgumentException($"未找到对应的 Ability 创建器: {triggerType} for {abilityData}");
            }

            return creator(abilityData);
        }
    }
}