using Combat.Ability;
using Combat.Systems;
using Core.Units;

namespace Combat.Effect.Effects
{
    public class DamageEffect : BaseEffect
    {
        private float DamageAmount { get; set; } = 0;

        public DamageEffect(EffectSpec spec) : base(spec)
        {
        }

        public override void SetContext(AbilityContext context)
        {
            base.SetContext(context);
            if (CheckValidContext())
            {
                DamageAmount = (float)context.Extra[0];
            }
        }

        public override void Execute()
        {
            var getSource = UnitManager.Instance.TryGetAvailableUnit(Context.SourceId, out var sourceUnit);
            if (!getSource) return;

            foreach (var unitId in Context.Targets.TaregetUnits)
            {
                if (UnitManager.Instance.TryGetAvailableUnit(unitId, out var targetUnit))
                {
                    CombatManager.Instance.DamageSystem.TryApplyDamage(targetUnit, DamageAmount, sourceUnit);
                }
            }
        }

        public override void Tick(float deltaTime)
        {
        }

        protected override bool CheckValidContext()
        {
            return base.CheckValidContext() && Context.Extra != null && Context.Extra.Length > 0;
        }
    }
}