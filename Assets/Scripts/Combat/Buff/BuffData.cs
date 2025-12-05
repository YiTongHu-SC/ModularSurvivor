namespace Combat.Buff
{
    /// <summary>
    /// Buff效果数据配置
    /// </summary>
    public class BuffData
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public BuffType Type { get; set; }
        public float Duration { get; set; }
        public float Value { get; set; }
        public bool CanStack { get; set; }
        
        public BuffData(int id, string name, BuffType type, float duration, float value, bool canStack = false)
        {
            ID = id;
            Name = name;
            Type = type;
            Duration = duration;
            Value = value;
            CanStack = canStack;
        }
    }
    
    /// <summary>
    /// Buff类型枚举
    /// </summary>
    public enum BuffType
    {
        /// <summary>增加移动速度</summary>
        SpeedBoost,
        /// <summary>减少移动速度</summary>
        SpeedReduction,
        /// <summary>增加攻击力</summary>
        AttackBoost,
        /// <summary>减少攻击力</summary>
        AttackReduction,
        /// <summary>持续伤害</summary>
        Poison,
        /// <summary>持续治疗</summary>
        Regeneration
    }
}