namespace Combat.Ability
{
    public class AbilityData
    {
        public int ID { get; set; }
        public AbilityType AbilityType;
    }

    public enum AbilityType
    {
        HitOnceOnCollision,
    }
}