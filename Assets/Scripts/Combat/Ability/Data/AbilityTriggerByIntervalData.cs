namespace Combat.Ability.Data
{
    public class AbilityTriggerByIntervalData : AbilityData
    {
        public float Interval;

        public AbilityTriggerByIntervalData()
        {
            TriggerType = TriggerType.Interval;
        }
    }
}