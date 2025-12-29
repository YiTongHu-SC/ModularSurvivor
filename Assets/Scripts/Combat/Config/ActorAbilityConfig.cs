using System;
using System.Collections.Generic;
using Combat.Ability.Data;
using Combat.Effect;
using UnityEngine;

namespace Combat.Config
{

    [CreateAssetMenu(fileName = "ActorAbilityConfig", menuName = "Combat Config/ActorAbilityConfig", order = 0)]
    public class ActorAbilityConfig : ScriptableObject
    {
        public string Name;
        public string Description;
        public TriggerType TriggerType;
        public FindTargetType FindTargetType;
        public float Cooldown;
        public float Interval;
        public float Cost;
        public EffectConfig EffectConfig;
        public List<ParamData> ExtraParams;
    }

    [Serializable]
    public struct ParamData
    {
        public ParamType ParamType;
        public string Value;
    }

    public enum ParamType
    {
        NONE = 0,
        TargetUnitId = 1, // 特定目标
        CollisionArea = 2, // 碰撞区域类型
        CollisionRadius = 3, // 范围半径
        CollisionSizeX = 4, // 碰撞尺寸X
        CollisionSizeY = 5, // 碰撞尺寸Y
        DamageAmount = 6, // 伤害数值
        HealAmount = 7, // 治疗数值
        ViewEventType = 8, // 视图事件类型
    }

    [Serializable]
    public class EffectConfig
    {
        public string Key;
        public string PreferenceKey;
        public float Delay; // 效果执行延迟时间（秒）
        public float Duration; // 效果持续时间（秒），0表示瞬时效果, -1表示无限持续
        public EffectNodeType EffectNodeType;
        public List<ParamData> ExtraParams;
        [SerializeReference]
        public List<EffectConfig> Children;
    }
}
