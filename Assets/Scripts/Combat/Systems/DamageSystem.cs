using Core.Events;
using Core.GameInterface;
using Core.Units;
using UnityEngine;

namespace Combat.Systems
{
    public class DamageSystem : ISystem
    {
        public void Reset()
        {
        }

        public void Tick(float deltaTime)
        {
        }

        public bool TryApplyDamage(UnitData target, float damageAmount, UnitData source = null)
        {
            if (target == null || damageAmount <= 0)
                return false;

            target.Health -= damageAmount;

            if (target.Health <= 0)
            {
                // Ensure health does not go below zero
                target.Health = 0;
                var sourceGUID = source?.RuntimeId ?? -1;
                EventManager.Instance.Publish(new GameEvents.UnitDeathEvent(target.RuntimeId, sourceGUID));
                Debug.Log($"Unit {target.RuntimeId} has killed by unit {sourceGUID}");
            }

            return true;
        }
    }
}