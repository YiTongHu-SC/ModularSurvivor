using Combat.Systems;

namespace Combat.Effect.Effects
{
    public abstract class BaseEffect : IEffectNode
    {
        public int NodeId { get; private set; }
        public EffectNodeType Type { get; } = EffectNodeType.Damage;
        public AbilityContext Context { get; set; }
        public bool IsComplete { get; } = false;

        public virtual void Initialize(AbilityContext context)
        {
            NodeId = CombatManager.Instance.GlobalAllocator.Next();
            Context = context;
        }

        // TODO: 实现效果逻辑, 例如造成伤害、治疗等, 后续重构为专门的处理管线
        public abstract void Execute();

        public abstract void Tick(float deltaTime);
    }
}