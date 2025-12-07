using Core.Events;
using UnityEngine;

namespace Combat.Buff
{
    public class BuffDelayDeath : Buff
    {
        public BuffDelayDeath(BuffData data, int targetUnitId) : base(data, targetUnitId)
        {
        }

        public override void ApplyEffect()
        {
        }

        public override void RemoveEffect()
        {
            EventManager.Instance.PublishEvent(new GameEvents.UnitDeathEvent(TargetUnitId, TargetUnitData.Position));
        }

        public override void UpdateEffect(float deltaTime)
        {
        }
    }
}