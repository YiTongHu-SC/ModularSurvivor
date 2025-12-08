using Core.Events;
using Core.Units;

namespace Combat.Systems
{
    public class DamageSystem
    {
        public void ApplyDamage(UnitData target, int damageAmount, UnitData source = null)
        {
            if (target == null || damageAmount <= 0)
                return;

            target.Health -= damageAmount;

            if (target.Health <= 0)
            {
                // Ensure health does not go below zero
                target.Health = 0;
                var sourceGUID = source?.GUID ?? -1;
                EventManager.Instance.PublishEvent(new GameEvents.UnitDeathEvent(target.GUID, sourceGUID));
            }
        }
    }
}