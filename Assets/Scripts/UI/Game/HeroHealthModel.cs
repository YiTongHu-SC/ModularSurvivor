using Core.Events;
using UI.Framework;

namespace UI.Game
{
    /// <summary>
    /// 英雄血量数据事件
    /// </summary>
    public class HeroHealthChangedEvent : EventData
    {
        public float OldHealth { get; set; }
        public float NewHealth { get; set; }
        public float MaxHealth { get; set; }
        public float HealthPercentage { get; set; }

        public HeroHealthChangedEvent()
        {
        }

        public HeroHealthChangedEvent(float oldHealth, float newHealth, float maxHealth)
        {
            OldHealth = oldHealth;
            NewHealth = newHealth;
            MaxHealth = maxHealth;
            HealthPercentage = maxHealth > 0 ? newHealth / maxHealth : 0f;
        }
    }

    /// <summary>
    /// 英雄血量数据结构
    /// </summary>
    public struct HeroHealthData
    {
        public float CurrentHealth { get; set; }
        public float MaxHealth { get; set; }
        public float HealthPercentage => MaxHealth > 0 ? CurrentHealth / MaxHealth : 0f;

        public HeroHealthData(float currentHealth, float maxHealth)
        {
            CurrentHealth = currentHealth;
            MaxHealth = maxHealth;
        }

        public override string ToString()
        {
            return $"Health: {CurrentHealth:F1}/{MaxHealth:F1} ({HealthPercentage:P1})";
        }
    }

    /// <summary>
    /// 英雄血量模型 - 集成EventManager的血量数据管理
    /// </summary>
    public class HeroHealthModel : BaseModel<HeroHealthData>
    {
        public override HeroHealthData Value { get; protected set; }
    }
}