using Combat.Ability.Data;

namespace Combat.Ability
{
    public class AbilityTriggerOnce : BaseAbility
    {
        public AbilityTriggerOnce(AbilityData data) : base(data)
        {
        }

        public override void ApplyAbility()
        {
            base.ApplyAbility();
            TryCastAbility();
            IsActive = false;
        }
    }
}