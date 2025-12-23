using Core.Units;

namespace Combat.Ability.Data
{
    public class LaserStrikeData : AbilityData
    {
        public int PresentationId;
        public float DamageAmount = 1.0f;
        public float HitCooldown = 1f;
        public float HitDuration = 0.2f;
        public UnitCollisionData collisionData;
    }
}