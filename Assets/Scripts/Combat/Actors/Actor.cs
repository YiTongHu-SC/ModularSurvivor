using Core.Events;
using Core.Units;

namespace Combat.Actors
{
    public class Actor : Unit
    {
        public override void OnEventReceived(GameEvents.UnitDeathEvent eventData)
        {
            base.OnEventReceived(eventData);
            if (eventData.GUID != GUID) return;
            // 处理角色死亡逻辑
            KillSelf();
        }
    }
}