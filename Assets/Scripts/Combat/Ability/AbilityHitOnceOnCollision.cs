namespace Combat.Ability
{
    public class AbilityHitOnceOnCollision : BaseAbility
    {
        public HitOnceOnCollisionData AbilityData { get; private set; }

        public AbilityHitOnceOnCollision(HitOnceOnCollisionData data, int targetId) : base(data, targetId)
        {
            AbilityData = data;
        }

        public override void ApplyAbility()
        {
        }

        public override void RemoveAbility()
        {
        }

        public override void UpdateAbility(float deltaTime)
        {
        }

        public override void PerformAbility()
        {
        }
    }
}