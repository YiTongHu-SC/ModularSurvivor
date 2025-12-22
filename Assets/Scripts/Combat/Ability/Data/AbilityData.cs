using Combat.Effect;

namespace Combat.Ability.Data
{
    public class AbilityData
    {
        public int RuntimeID;
        public string GUID;
        public string Key;
        public int SourceId = -1;
        public TriggerType TriggerType;
        public FindTargetType FindTargetType;
        public float Cost;
        public float Cooldown;
        public EffectSpec EffectSpec;
        public object[] ExtraParams;
    }

    public enum FindTargetType
    {
        Self, // 自己
        Ally, // 友方单位
        Enemy, // 敌方单位
        Area, // 区域内单位
        Global, // 全局单位
        Specific // 指定单位
    }

    public enum TriggerType
    {
        Active, // 主动技能
        Passive, // 被动技能
        Interval, // 间隔触发技能
        ByEvent, // 事件触发技能
        Once, // 只触发一次的技能
        PlayerInput, // 玩家输入控制
    }

    public enum TriggerEventType
    {
        OnCollideOtherUnit, // 击中其他单位时
    }
}