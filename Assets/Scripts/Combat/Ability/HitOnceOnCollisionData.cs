namespace Combat.Ability
{
    public class HitOnceOnCollisionData : AbilityData
    {
        public float DamageAmount { get; private set; }

        public HitOnceOnCollisionData(float damageAmount)
        {
            DamageAmount = damageAmount;
        }
    }
}