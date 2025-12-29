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

        public class DebugActorEvent : EventData
        {
            public DebugActorAction DebugActorAction { get; }
            public string Value { get; }

            public DebugActorEvent(DebugActorAction debugActorAction, string value = "")
            {
                DebugActorAction = debugActorAction;
                Value = value;
            }
        }

        public enum DebugActorAction
        {
            CheckActor,
        }
    }
}