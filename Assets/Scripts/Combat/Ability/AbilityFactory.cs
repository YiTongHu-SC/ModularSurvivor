using System;
using System.Collections.Generic;
using Combat.Ability.Data;

namespace Combat.Ability
{
    public static class AbilityFactory
    {
        // key: BuffId，value: 创建该 Buff 的委托
        private static readonly Dictionary<TriggerType, Func<AbilityData, int, Ability>> _creators
            = new()
            {
                {
                    TriggerType.HitOnceOnCollision,
                    (data, targetId) => new Ability(data as HitOnceOnCollisionData, targetId)
                },
                {
                    TriggerType.LaserStrike,
                    (data, targetId) => new Ability(data as LaserStrikeData, targetId)
                },
                {
                    TriggerType.PlayerInput,
                    (data, targetId) => new Ability(data as PlayerInputData, targetId)
                },
                {
                    TriggerType.ChaseHero,
                    (data, targetId) => new Ability(data, targetId)
                },
            };

        public static Ability CreateAbility(TriggerType triggerType, AbilityData abilityData, int unitId)
        {
            if (!_creators.TryGetValue(triggerType, out var creator))
            {
                throw new ArgumentException($"未找到对应的 Ability 创建器: {triggerType} for unit {unitId}");
            }

            return creator(abilityData, unitId);
        }
    }
}