using Core.Units;

namespace Combat.Ability
{
    public class LaserStrikeData : AbilityData
    {
        public float DamageAmount = 1.0f;
        public float HitCooldown = 1f;
        public float HitDuration = 0.2f;
        public UnitCollisionData collisionData;
    }
}