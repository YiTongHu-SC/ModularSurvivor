using Core.Units;

namespace Combat.Ability.Data
{
    public class AbilityChaseHero : BaseAbility
    {
        public AbilityChaseHero(AbilityData abilityData, int id) : base(abilityData, id)
        {
        }

        public override void ApplyAbility()
        {
        }

        public override void RemoveAbility()
        {
        }

        public override void UpdateAbility(float deltaTime)
        {
            UnitData.MovementContext.targetPosition = UnitManager.Instance.HeroUnitData.Position;
        }

        public override void PerformAbility()
        {
        }
    }
}