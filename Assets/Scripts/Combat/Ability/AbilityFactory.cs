using System;
using System.Collections.Generic;
using Combat.Ability.Data;

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
                {
                    AbilityType.LaserStrike,
                    (data, targetId) => new AbilityLaserStrike(data as LaserStrikeData, targetId)
                },
                {
                    AbilityType.PlayerInput,
                    (data, targetId) => new AbilityPlayerInput(data as PlayerInputData, targetId)
                },
                {
                    AbilityType.ChaseHero,
                    (data, targetId) => new AbilityChaseHero(data, targetId)
                },
            };

        public static BaseAbility CreateAbility(AbilityType abilityType, AbilityData abilityData, int unitId)
        {
            if (!_creators.TryGetValue(abilityType, out var creator))
            {
                throw new ArgumentException($"未找到对应的 Ability 创建器: {abilityType} for unit {unitId}");
            }

            return creator(abilityData, unitId);
        }
    }
}