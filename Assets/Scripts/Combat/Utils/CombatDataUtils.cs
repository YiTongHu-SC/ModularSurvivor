using Combat.Buff;
using Core.AssetsTool;
using UnityEngine;

namespace Combat.Utils
{
    public static class CombatDataUtils
    {
        public static BuffData CreateBuffDataByKey(string buffKey)
        {
            var handle = AssetSystem.Instance.LevelScope.Acquire<BuffDataConfig>(buffKey);
            if (handle.IsValid)
            {
                var buffConfig = handle.Asset;
                var buffData = new BuffData(
                    0,
                    buffConfig.Name,
                    buffConfig.BuffType,
                    buffConfig.Duration,
                    buffConfig.Value,
                    buffConfig.CanStack);
                return buffData;
            }
            else
            {
                Debug.LogError($"无法加载Buff配置: {buffKey}");
                return null;
            }
        }
    }
}