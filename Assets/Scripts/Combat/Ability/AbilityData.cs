namespace Combat.Ability
{
    public class AbilityData
    {
        public int ID { get; set; }
        public AbilityType AbilityType;
    }

    public enum AbilityType
    {
        HitOnceOnCollision, // 碰撞时造成一次伤害
        LaserStrike // 激光打击
    }
}