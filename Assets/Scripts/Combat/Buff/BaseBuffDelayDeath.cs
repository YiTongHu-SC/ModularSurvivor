using Core.Events;
using UnityEngine;

namespace Combat.Buff
{
    public class BaseBuffDelayDeath : BaseBuff
    {
        public BaseBuffDelayDeath(BuffData data, int targetUnitId) : base(data, targetUnitId)
        {
        }

        public override void ApplyEffect()
        {
        }

        public override void RemoveEffect()
        {
            EventManager.Instance.PublishEvent(new GameEvents.UnitDeathEvent(TargetUnitId));
        }

        public override void UpdateEffect(float deltaTime)
        {
        }
    }
}