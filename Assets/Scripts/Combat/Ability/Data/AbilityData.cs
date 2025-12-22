using Combat.Effect;

namespace Combat.Ability.Data
{
    public class AbilityData
    {
        public int RuntimeID;
        public string GUID;
        public string Key;
        public int TargetID = -1;
        public TriggerType TriggerType;
        public float Cost;
        public float Cooldown;
        public EffectSpec[] Effects;
    }

    public enum TriggerType
    {
        Active, // 主动技能
        Passive, // 被动技能
        Interval, // 间隔触发技能
        ByEvent, // 事件触发技能

        /// 废弃
        HitOnceOnCollision, // 碰撞时造成一次伤害
        LaserStrike, // 激光打击
        PlayerInput, // 玩家输入控制
        ChaseHero, // 追逐英雄
    }
}