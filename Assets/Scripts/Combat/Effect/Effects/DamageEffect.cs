namespace Combat.Effect.Effects
{
    public class DamageEffect : IEffectNode
    {
        public EffectNodeType Type { get; } = EffectNodeType.Damage;
        public AbilityContext Context { get; set; }
        public bool IsComplete { get; } = false;

        public void Initialize(AbilityContext context)
        {
            Context = context;
        }

        public void Execute()
        {
        }

        public void Tick(float deltaTime)
        {
        }
    }
}