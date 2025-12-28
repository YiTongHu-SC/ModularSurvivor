using System;
using System.Collections.Generic;

namespace Combat.Buff
{
    public static class BuffFactory
    {
        // key: BuffId，value: 创建该 Buff 的委托
        private static readonly Dictionary<BuffType, Func<BuffData, int, BaseBuff>> _creators
            = new()
            {
                { BuffType.SpeedBoost, (data, targetId) => new BuffSpeedBoost(data, targetId) },
                { BuffType.DelayDeath, (data, targetId) => new BuffDelayDeath(data, targetId) },
            };

        public static BaseBuff CreateBuff(BuffType id, BuffData data, int targetUnitId)
        {
            if (!_creators.TryGetValue(id, out var creator))
            {
                throw new ArgumentException($"未找到对应的 Buff 创建器: {id}");
            }

            return creator(data, targetUnitId);
        }
    }
}