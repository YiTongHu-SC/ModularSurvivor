using System;
using Combat.Systems;
using Core.Units;

namespace Combat.Effect.Effects
{
    public class BuffEffect : BaseEffect
    {
        public BuffEffect(EffectSpec effectSpec) : base(effectSpec)
        {
        }

        public override void Execute()
        {
            ApplyBuff();
        }

        private void ApplyBuff()
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
                    // Apply buff here
                    foreach (var buffKey in Spec.BuffKeys)
                    {
                        CombatManager.Instance.BuffSystem.ApplyBuffByKey(buffKey, targetUnit.RuntimeId);
                    }
                }
            }
        }
    }
}