using Core.Events;
using Core.Units;
using UnityEngine;

namespace Combat.Systems
{
    public class DamageSystem
    {
        public bool TryApplyDamage(UnitData target, float damageAmount, UnitData source = null)
        {
            if (target == null || damageAmount <= 0)
                return false;

            target.Health -= damageAmount;

            if (target.Health <= 0)
            {
                // Ensure health does not go below zero
                target.Health = 0;
                var sourceGUID = source?.GUID ?? -1;
                EventManager.Instance.Publish(new GameEvents.UnitDeathEvent(target.GUID, sourceGUID));
                Debug.Log($"Unit {target.GUID} has killed by unit {sourceGUID}");
            }

            return true;
        }
    }
}