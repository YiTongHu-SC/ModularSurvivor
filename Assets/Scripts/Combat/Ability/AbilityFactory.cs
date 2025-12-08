using System;
using System.Collections.Generic;

namespace Combat.Ability
{
    public static class AbilityFactory
    {
        // key: BuffId，value: 创建该 Buff 的委托
        private static readonly Dictionary<AbilityType, Func<AbilityData, int, BaseAbility>> _creators
            = new()
            {
                {
                    AbilityType.HitOnceOnCollision,
                    (data, targetId) => new AbilityHitOnceOnCollision(data as HitOnceOnCollisionData, targetId)
                },
            };

        public static BaseAbility CreateAbility(AbilityType abilityType, AbilityData abilityData, int unitId)
        {
            if (!_creators.TryGetValue(abilityType, out var creator))
            {
                throw new ArgumentException($"未找到对应的 Buff 创建器: {unitId}");
            }

            return creator(abilityData, unitId);
        }
    }
}