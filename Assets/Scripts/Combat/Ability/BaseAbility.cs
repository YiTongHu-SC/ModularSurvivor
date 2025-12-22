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
        protected AbilityContext Context { get; private set; }
        protected TargetSet Targets { get; private set; }
        protected bool IsOnCooldown { get; set; } = false;
        private float CooldownTimer { get; set; } = 0;

        protected BaseAbility(AbilityData data)
        {
            IsActive = true;
            UnitId = data.TargetID;
            AbilityData = data;
            Targets = new TargetSet();
            Context = new AbilityContext(UnitId, Targets);
        }

        /// <summary>
        /// 应用能力效果，新增能力时调用一次
        /// </summary>
        public virtual void ApplyAbility()
        {
            IsOnCooldown = false;
            CooldownTimer = 0;
        }

        protected virtual void TryCastAbility()
        {
            if (!IsActive || IsOnCooldown) return;
            Context.Extra = AbilityData.EffectSpec.EffectParams;
            var effect = EffectFactory.CreateEffectNode(AbilityData.EffectSpec);
            effect.SetContext(Context);
            CombatManager.Instance.EffectSystem.CastEffect(effect);
            IsOnCooldown = true;
            CooldownTimer = 0;
        }

        /// <summary>
        /// 移除能力效果，能力结束时调用一次
        /// </summary>
        public virtual void RemoveAbility()
        {
            IsActive = false;
            Targets = null;
            Context = null;
            UnitId = 0;
        }

        /// <summary>
        /// 更新能力状态，每帧调用
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        public void TickAbility(float deltaTime)
        {
            // 更新冷却时间
            if (IsOnCooldown)
            {
                CooldownTimer += deltaTime;
                if (CooldownTimer >= AbilityData.Cooldown)
                {
                    IsOnCooldown = false;
                    CooldownTimer = 0;
                }
            }
        }
    }
}