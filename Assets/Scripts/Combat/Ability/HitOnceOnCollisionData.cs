namespace Combat.Ability
{
    public class HitOnceOnCollisionData : AbilityData
    {
        public float DamageAmount { get; private set; } = 1.0f;
        public float HitCooldown { get; private set; } = 0.2f;
    }
}