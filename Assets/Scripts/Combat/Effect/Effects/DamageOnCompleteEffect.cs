using System;
using Combat.Ability;
using Combat.Systems;
using Core.Units;

namespace Combat.Effect.Effects
{
    public class DamageOnCompleteEffect : BaseEffect
    {
        private float DamageAmount { get; set; } = 0;

        public DamageOnCompleteEffect(EffectSpec effectSpec) : base(effectSpec)
        {
            EffectType = EffectNodeType.DamageOnComplete;
        }

        public override void SetContext(AbilityContext context)
        {
            base.SetContext(context);
            if (CheckValidContext())
            {
                DamageAmount = (float)Spec.EffectParams["DamageAmount"];
            }
        }

        public override void Execute()
        {
        }

        public override bool TryCast(Action onExecute = null)
        {
            if (!CheckValidTarget())
            {
                IsComplete = true;
                return false;
            }

            return base.TryCast(onExecute);
        }

        protected override void OnComplete()
        {
            base.OnComplete();
            ApplyDamage();
        }

        private void ApplyDamage()
        {
            var getSource = UnitManager.Instance.TryGetAvailableUnit(Context.SourceId, out var sourceUnit);
            if (!getSource)
            {
                return;
            }

            foreach (var unitId in Context.Targets.TargetUnits)
            {
                if (UnitManager.Instance.TryGetAvailableUnit(unitId, out var targetUnit))
                {
                    CombatManager.Instance.DamageSystem.TryApplyDamage(targetUnit, DamageAmount, sourceUnit);
                }
            }
        }
    }
}