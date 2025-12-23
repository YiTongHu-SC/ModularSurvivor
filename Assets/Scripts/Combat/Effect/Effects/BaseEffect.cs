using System;
using Combat.Ability;
using Combat.Systems;
using Core.Events;
using Core.Timer;
using Core.Units;

namespace Combat.Effect.Effects
{
    public abstract class BaseEffect : IEffectNode
    {
        public int NodeId { get; private set; }
        public EffectNodeType Type { get; } = EffectNodeType.Damage;
        public AbilityContext Context { get; set; }
        public EffectSpec Spec { get; set; }
        public bool IsComplete { get; protected set; } = false;
        private float _elapsedTime = 0f;

        protected BaseEffect(EffectSpec effectSpec)
        {
            NodeId = CombatManager.Instance.GlobalAllocator.Next();
            Spec = effectSpec;
            IsComplete = false;
            _elapsedTime = 0f;
        }

        public virtual void SetContext(AbilityContext context)
        {
            Context = context;
        }

        // TODO: 实现效果逻辑, 例如造成伤害、治疗等, 后续重构为专门的处理管线
        public abstract void Execute();

        public virtual bool TryCast(Action onExecute = null)
        {
            if (!CheckValidContext())
            {
                IsComplete = true;
                return false;
            }

            TimeManager.Instance.TimeSystem.CreateTimer(Spec.Delay, () =>
            {
                Execute();
                onExecute?.Invoke();
            });
            // 发布视图更新事件
            if (Spec.EffectParams != null &&
                Spec.EffectParams.TryGetValue("ViewData", out var param))
            {
                if (param is ViewBaseData viewData)
                {
                    viewData.Delay = Spec.Delay;
                    viewData.Duration = Spec.Duration;
                    viewData.SourceId = Context.SourceId;
                    if (Context.Targets.TargetUnits != null)
                    {
                        viewData.TargetIds = Context.Targets.TargetUnits; // 仅支持单目标视图
                    }

                    EventManager.Instance.Publish(
                        new GameEvents.UpdatePreferenceEvent(Spec.PreferenceKey, viewData));
                }
            }

            return true;
        }

        public virtual void Tick(float deltaTime)
        {
            if (Spec.Duration < 0) return;
            _elapsedTime += deltaTime;
            if (_elapsedTime >= Spec.Duration)
            {
                OnComplete();
            }
        }

        // 效果完成时的处理逻辑
        protected virtual void OnComplete()
        {
            IsComplete = true;
        }

        public virtual void Remove()
        {
            NodeId = 0;
            Context = null;
            Spec = null;
        }

        protected virtual bool CheckValidContext()
        {
            return Context != null;
        }

        protected virtual bool CheckValidTarget()
        {
            return Context.Targets != null && Context.Targets.TargetUnits.Count > 0;
        }
    }
}