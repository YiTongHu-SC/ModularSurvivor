using Core.Units;

namespace Combat.Effect.Effects
{
    public class ChaseTargetEffect : BaseEffect
    {
        private int TargetId { get; set; } = -1;

        public ChaseTargetEffect(EffectSpec effectSpec) : base(effectSpec)
        {
            EffectType = EffectNodeType.ChaseTarget;
        }

        public override void Execute()
        {
            ChaseTarget();
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            ChaseTarget();
        }

        private void ChaseTarget()
        {
            var getSource = UnitManager.Instance.TryGetAvailableUnit(Context.SourceId, out var sourceUnit);
            if (!getSource)
            {
                IsComplete = true;
                return;
            }

            if (!CheckValidTarget())
            {
                IsComplete = true;
                return;
            }

            TargetId = Context.Targets.TargetUnits[0];
            if (!UnitManager.Instance.CheckUnitAvailability(TargetId))
            {
                IsComplete = true;
                return;
            }

            if (UnitManager.Instance.TryGetAvailableUnit(TargetId, out var unit))
            {
                sourceUnit.MovementContext.targetPosition = unit.Position;
            }
        }
    }
}