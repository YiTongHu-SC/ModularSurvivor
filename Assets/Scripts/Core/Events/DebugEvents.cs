namespace Core.Events
{
    public class DebugEvents
    {
        public class ApplyDamageEvent : EventData
        {
            public int TargetId { get; }
            public int DamageAmount { get; }

            public ApplyDamageEvent(int targetId, int damageAmount)
            {
                TargetId = targetId;
                DamageAmount = damageAmount;
            }
        }
    }
}