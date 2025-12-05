using System;
using Core.Events;
using Core.Units;

namespace Combat.Actors
{
    public class Actor : Unit
    {
        // public ActorConfig Config;
        public override void Initialize(UnitData data)
        {
            base.Initialize(data);
        }

        public override void OnEventReceived(EventData eventData)
        {
        }
    }
}