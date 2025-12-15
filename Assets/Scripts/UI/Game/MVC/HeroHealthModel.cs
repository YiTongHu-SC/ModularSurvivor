using Core.Events;
using UI.Framework;

namespace UI.Game.MVC
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
        
        public HeroHealthChangedEvent() { }
        
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
    public class HeroHealthModel : EventDrivenModel<HeroHealthData, HeroHealthChangedEvent>
    {
        private HeroHealthData _value;
        
        /// <summary>
        /// 血量数据
        /// </summary>
        public override HeroHealthData Value 
        { 
            get => _value;
            protected set => _value = value;
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="initialHealth">初始生命值</param>
        /// <param name="maxHealth">最大生命值</param>
        public HeroHealthModel(float initialHealth, float maxHealth)
        {
            _value = new HeroHealthData(initialHealth, maxHealth);
        }
        
        /// <summary>
        /// 设置当前血量
        /// </summary>
        /// <param name="currentHealth">当前血量</param>
        public void SetCurrentHealth(float currentHealth)
        {
            var newData = new HeroHealthData(currentHealth, _value.MaxHealth);
            SetValue(newData);
        }
        
        /// <summary>
        /// 设置最大血量
        /// </summary>
        /// <param name="maxHealth">最大血量</param>
        public void SetMaxHealth(float maxHealth)
        {
            var newData = new HeroHealthData(_value.CurrentHealth, maxHealth);
            SetValue(newData);
        }
        
        /// <summary>
        /// 设置完整血量数据
        /// </summary>
        /// <param name="currentHealth">当前血量</param>
        /// <param name="maxHealth">最大血量</param>
        public void SetHealthData(float currentHealth, float maxHealth)
        {
            var newData = new HeroHealthData(currentHealth, maxHealth);
            SetValue(newData);
        }
        
        /// <summary>
        /// 创建血量变更事件
        /// </summary>
        /// <param name="oldValue">旧值</param>
        /// <param name="newValue">新值</param>
        /// <returns>血量变更事件</returns>
        protected override HeroHealthChangedEvent CreateEvent(HeroHealthData oldValue, HeroHealthData newValue)
        {
            return new HeroHealthChangedEvent(
                oldValue.CurrentHealth,
                newValue.CurrentHealth,
                newValue.MaxHealth
            );
        }
    }
}
