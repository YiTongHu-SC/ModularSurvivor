using Combat.Systems;
using Core.Units;

namespace Combat.Effect.Effects
{
    public class DamageEffect : BaseEffect
    {
        const int DAMAGE_AMOUNT = 100;

        public override void Execute()
        {
            var getSource = UnitManager.Instance.TryGetAvailableUnit(Context.SourceId, out var sourceUnit);
            if (!getSource) return;

            foreach (var unitId in Context.Targets.Units)
            {
                if (UnitManager.Instance.TryGetAvailableUnit(unitId, out var targetUnit))
                {
                    CombatManager.Instance.DamageSystem.TryApplyDamage(targetUnit, DAMAGE_AMOUNT, sourceUnit);
                }
            }
        }

        public override void Tick(float deltaTime)
        {
        }
    }
}