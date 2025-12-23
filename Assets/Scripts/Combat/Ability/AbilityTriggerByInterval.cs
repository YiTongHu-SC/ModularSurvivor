using Combat.Ability.Data;

namespace Combat.Ability
{
    public class AbilityTriggerByInterval : BaseAbility
    {
        private AbilityTriggerByIntervalData AbilityDataOverride { get; set; }

        private float _timer;

        public AbilityTriggerByInterval(AbilityTriggerByIntervalData data) : base(data)
        {
            AbilityDataOverride = data as AbilityTriggerByIntervalData;
        }

        public override void ApplyAbility()
        {
            base.ApplyAbility();
            // 这里可以添加定时触发能力的逻辑，例如使用协程或计时器
            // 每隔 AbilityDataOverride.Interval 秒调用 TryCastAbility()
            _timer = 0f;
        }

        public override void RemoveAbility()
        {
            base.RemoveAbility();
        }

        public override void TickAbility(float deltaTime)
        {
            base.TickAbility(deltaTime);
            if (!IsActive || IsOnCooldown) return;
            _timer += deltaTime;
            if (_timer >= AbilityDataOverride.Interval)
            {
                TryCastAbility();
                _timer = 0f;
            }
        }
    }
}