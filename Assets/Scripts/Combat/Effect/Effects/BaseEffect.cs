using Combat.Ability;
using Combat.Systems;

namespace Combat.Effect.Effects
{
    public abstract class BaseEffect : IEffectNode
    {
        public int NodeId { get; private set; }
        public EffectNodeType Type { get; } = EffectNodeType.Damage;
        public AbilityContext Context { get; set; }
        public EffectSpec Spec { get; set; }
        public bool IsComplete { get; protected set; } = false;

        protected BaseEffect(EffectSpec spec)
        {
            Initialize(spec);
        }
        
        public virtual void Initialize(EffectSpec effectSpec)
        {
            NodeId = CombatManager.Instance.GlobalAllocator.Next();
            Spec = effectSpec;
            IsComplete = false;
        }

        public virtual void SetContext(AbilityContext context)
        {
            Context = context;
        }


        // TODO: 实现效果逻辑, 例如造成伤害、治疗等, 后续重构为专门的处理管线
        public abstract void Execute();

        public abstract void Tick(float deltaTime);

        protected virtual bool CheckValidContext()
        {
            return Context != null;
        }
    }
}