using Combat.Ability.Data;
using Combat.Effect;
using Combat.Systems;

namespace Combat.Ability
{
    public abstract class BaseAbility
    {
        public int UnitId { get; private set; }
        public AbilityData AbilityData { get; private set; }
        public bool IsActive { get; set; } = false;
        public bool IsExpired => !IsActive;

        public BaseAbility(AbilityData data)
        {
            IsActive = true;
            UnitId = data.TargetID;
            AbilityData = data;
        }

        /// <summary>
        /// 应用能力效果，新增能力时调用一次
        /// </summary>
        public void ApplyAbility()
        {
            foreach (var effectSpec in AbilityData.Effects)
            {
                var effect = EffectFactory.CreateEffectNode(effectSpec);
                CombatManager.Instance.EffectSystem.ApplyEffect(effect);
            }
        }

        /// <summary>
        /// 移除能力效果，能力结束时调用一次
        /// </summary>
        public void RemoveAbility()
        {
        }

        /// <summary>
        /// 更新能力状态，每帧调用
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        public void TickAbility(float deltaTime)
        {
        }
    }
}