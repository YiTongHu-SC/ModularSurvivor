using System.Collections.Generic;
using System.Linq;
using Combat.Ability;
using Combat.Ability.Data;
using Core.Events;
using Core.Units;
using UnityEngine;

namespace Combat.Systems
{
    public class AbilitySystem
    {
        // 按单位ID存储Buff列表
        private readonly Dictionary<int, List<BaseAbility>> _unitAbilities = new();

        public void Initialize()
        {
        }

        public bool ApplyAbility(AbilityType abilityType, AbilityData abilityData, int unitId)
        {
            // 检查单位是否存在
            if (!UnitManager.Instance.Units.ContainsKey(unitId))
            {
                Debug.LogWarning($"未找到单位: {unitId}");
                return false;
            }

            //
            // // 确保单位有Buff列表
            if (!_unitAbilities.ContainsKey(unitId))
            {
                _unitAbilities[unitId] = new List<BaseAbility>();
            }

            //
            var ability = AbilityFactory.CreateAbility(abilityType, abilityData, unitId);
            var unitAbility = _unitAbilities[unitId];

            // 添加新Buff
            unitAbility.Add(ability);
            ability.ApplyAbility();
            Debug.Log($"应用能力: {ability.AbilityData.AbilityType} 到单位 {unitId}");

            return true;
        }

        public bool RemoveAbility(int unitId, int abilityId)
        {
            if (!_unitAbilities.TryGetValue(unitId, out var unitBuffs))
                return false;
            var abilityRemove = unitBuffs.FirstOrDefault(b => b.AbilityData.ID == abilityId && b.IsActive);
            if (abilityRemove == null)
                return false;
            abilityRemove.RemoveAbility();
            abilityRemove.IsActive = false;
            Debug.Log($"移除能力: {abilityRemove.AbilityData.AbilityType} 从单位 {unitId}");
            return true;
        }

        public void UpdateAbilities(float deltaTime)
        {
            var expiredAbilities = new List<(int unitId, BaseAbility ability)>();
            foreach (var unitAbilities in _unitAbilities)
            {
                var unitId = unitAbilities.Key;
                var unitAbilitiesList = unitAbilities.Value;
                if (!UnitManager.Instance.CheckUnitAvailability(unitId))
                {
                    // 如果单位不可用，标记所有能力为过期
                    foreach (var ability in unitAbilitiesList)
                    {
                        expiredAbilities.Add((unitId, ability));
                    }

                    continue;
                }

                for (int i = unitAbilitiesList.Count - 1; i >= 0; i--)
                {
                    var ability = unitAbilitiesList[i];
                    // 这里假设有某种条件判断能力是否过期
                    if (!ability.IsActive)
                    {
                        expiredAbilities.Add((unitId, ability));
                    }
                    else
                    {
                        ability.UpdateAbility(deltaTime);
                    }
                }
            }

            // 移除过期的能力
            foreach (var (unitId, ability) in expiredAbilities)
            {
                ability.RemoveAbility();
                _unitAbilities[unitId].Remove(ability);
                Debug.Log($"能力过期并移除: {ability.AbilityData.AbilityType} 单位 {unitId}");
                EventManager.Instance.Publish(
                    new GameEvents.AbilityRemovedEvent(unitId, ability.AbilityData.ID));
            }
        }
    }
}