using System;
using System.Collections.Generic;
using Combat.Ability.Data;
using UnityEngine;

namespace Combat.Ability
{
    public class AbilitySystem
    {
        // 按单位ID存储Buff列表
        private readonly Dictionary<int, BaseAbility> _abilities = new();
        private readonly List<int> _expiredAbilities = new();

        public void Initialize()
        {
        }

        internal void Reset()
        {
            _abilities.Clear();
            _expiredAbilities.Clear();
        }

        public bool ApplyAbility(AbilityData abilityData)
        {
            var ability = AbilityFactory.CreateAbility(abilityData);
            _abilities.TryAdd(ability.AbilityData.RuntimeID, ability);
            ability.ApplyAbility();
            Debug.Log($"应用能力: {ability.AbilityData.Key} to unit {ability.UnitId}");
            return true;
        }

        public void TickAbilities(float deltaTime)
        {
            _expiredAbilities.Clear();
            foreach (var ability in _abilities.Values)
            {
                if (ability.IsExpired)
                {
                    _expiredAbilities.Add(ability.AbilityData.RuntimeID);
                }
                else
                {
                    ability.TickAbility(deltaTime);
                }
            }

            // 移除过期的能力
            foreach (var abilityId in _expiredAbilities)
            {
                Debug.Log($"能力过期并移除: {_abilities[abilityId].AbilityData.Key}");
                _abilities[abilityId].RemoveAbility();
                _abilities.Remove(abilityId);
            }
        }


    }
}