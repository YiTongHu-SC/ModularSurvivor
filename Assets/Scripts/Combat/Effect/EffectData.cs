using System;
using System.Collections.Generic;

namespace Combat.Effect
{
    [Serializable]
    public class EffectSpec
    {
        public string GUID; // 效果节点ID（配置用）
        public string Key; // 效果节点Key（配置用）
        public EffectNodeType EffectNodeType;
        public List<string> tags;

        public float Delay; // 效果执行延迟时间（秒）

        // 通用：子节点
        public List<EffectSpec> children;

        // 通用参数（最简可先用 Dictionary 或强类型 Params）
        public Dictionary<string, object> EffectParams;

        // public ConditionSpec condition;   // If 的条件/过滤（可选）

        // 表现相关
        public string PreferenceKey;
    }

    public enum EffectNodeType
    {
        /// 效果
        Damage = 10000, // apply damage to target directly
        Heal = 10001, // restore health to target directly
        ApplyBuff = 10002,
        RemoveBuff = 10003,
        SpawnProjectile = 10004,
        Knockback = 10005,
        Stun = 10006,
        Slow = 10007,
        SpeedBoost = 10008,
        ChaseTarget = 10009,

        /// 组合方式
        Chain = 20000,
        Parallel = 20001,
        Delay = 20002,
        Repeat = 20003,
        If = 20004,
        Chance = 20005,
        Fork = 20006,
    }
}