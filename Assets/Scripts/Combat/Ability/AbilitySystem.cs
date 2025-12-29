using System;
using System.Collections.Generic;
using Combat.Ability.Data;
using Core.GameInterface;
using UnityEngine;

namespace Combat.Ability
{
    public class AbilitySystem : ISystem
    {
        /// <summary>
        /// 已应用的能力列表, key: AbilityData.RuntimeID
        /// </summary>
        private readonly Dictionary<int, BaseAbility> _abilities = new();
        private readonly Dictionary<int, List<BaseAbility>> _actorAbilities = new();
        private readonly List<int> _expiredAbilities = new();
        public Dictionary<int, List<BaseAbility>> ActorAbilities => _actorAbilities;
        public void Reset()
        {
            _abilities.Clear();
            _expiredAbilities.Clear();
            _actorAbilities.Clear();
        }

        public bool ApplyAbility(AbilityData abilityData)
        {
            var ability = AbilityFactory.CreateAbility(abilityData);
            _abilities.TryAdd(ability.AbilityData.RuntimeID, ability);
            ability.ApplyAbility();
            Debug.Log($"应用能力: {ability.AbilityData.Key} to unit {ability.UnitId}");
            if (!_actorAbilities.ContainsKey(ability.UnitId))
            {
                _actorAbilities[ability.UnitId] = new List<BaseAbility>();
            }
            _actorAbilities[ability.UnitId].Add(ability);
            return true;
        }

        /// <summary>
        /// 每帧更新能力系统逻辑
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Tick(float deltaTime)
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
                if (_actorAbilities.ContainsKey(_abilities[abilityId].UnitId))
                {
                    _actorAbilities[_abilities[abilityId].UnitId].Remove(_abilities[abilityId]);
                }
                _abilities.Remove(abilityId);
            }
        }
    }
}