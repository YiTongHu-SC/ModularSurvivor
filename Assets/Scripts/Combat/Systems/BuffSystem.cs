using System;
using System.Collections.Generic;
using System.Linq;
using Combat.Buff;
using Core.Events;
using Core.Units;
using UnityEngine;

namespace Combat.Systems
{
    /// <summary>
    /// Buff系统，管理所有单位的增益和减益效果
    /// </summary>
    public class BuffSystem
    {
        // 按单位ID存储Buff列表
        private readonly Dictionary<int, List<BaseBuff>> _unitBuffs = new();

        public void Initialize()
        {
        }

        /// <summary>
        /// 应用Buff到指定单位
        /// </summary>
        /// <param name="buffType">Buff模板ID</param>
        /// <param name="buffData"></param>
        /// <param name="unitId">目标单位ID</param>
        /// <returns>是否成功应用</returns>
        public bool ApplyBuff(BuffType buffType, BuffData buffData, int unitId)
        {
            // 检查单位是否存在
            if (!UnitManager.Instance.Units.ContainsKey(unitId))
            {
                Debug.LogWarning($"未找到单位: {unitId}");
                return false;
            }

            // 确保单位有Buff列表
            if (!_unitBuffs.ContainsKey(unitId))
            {
                _unitBuffs[unitId] = new List<BaseBuff>();
            }

            var newBuff = BuffFactory.CreateBuff(buffType, buffData, unitId);
            var unitBuffs = _unitBuffs[unitId];

            // 尝试叠加现有Buff
            var existingBuff = unitBuffs.FirstOrDefault(b => b.Data.BuffType == buffType && b.IsActive);
            if (existingBuff != null && existingBuff.TryStack(newBuff))
            {
                Debug.Log($"Buff叠加成功: {existingBuff.Data.BuffType} (层数: {existingBuff.StackCount})");
            }
            else
            {
                // 添加新Buff
                unitBuffs.Add(newBuff);
                Debug.Log($"应用新Buff: {newBuff.Data.BuffType} 到单位 {unitId}");
            }

            // 应用Buff效果
            newBuff.ApplyEffect();
            // 发布事件
            EventManager.Instance.Publish(new GameEvents.BuffAppliedEvent(newBuff.Data.ID, newBuff.TargetUnitId));

            return true;
        }

        /// <summary>
        /// 移除指定单位的指定Buff
        /// </summary>
        /// <param name="unitId">单位ID</param>
        /// <param name="buffId">Buff ID</param>
        /// <returns>是否成功移除</returns>
        public bool RemoveBuff(int unitId, int buffId)
        {
            if (!_unitBuffs.TryGetValue(unitId, out var unitBuffs))
                return false;

            var buffToRemove = unitBuffs.FirstOrDefault(b => b.Data.ID == buffId && b.IsActive);
            if (buffToRemove == null)
                return false;

            // 移除Buff效果
            buffToRemove.RemoveEffect();

            // 标记为非活跃
            buffToRemove.IsActive = false;

            // 发布事件
            EventManager.Instance.Publish(new GameEvents.BuffRemovedEvent(unitId, buffId, buffToRemove.Data.Name));

            return true;
        }

        /// <summary>
        /// 更新所有Buff（每帧调用）
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        public void UpdateBuffs(float deltaTime)
        {
            var expiredBuffs = new List<(int unitId, Combat.Buff.BaseBuff buff)>();

            foreach (var kvp in _unitBuffs)
            {
                var unitId = kvp.Key;
                var buffs = kvp.Value;

                for (int i = buffs.Count - 1; i >= 0; i--)
                {
                    var buff = buffs[i];

                    if (!buff.UpdateTime(deltaTime))
                    {
                        // Buff过期
                        expiredBuffs.Add((unitId, buff));
                        buffs.RemoveAt(i);
                    }
                    else
                    {
                        // 处理持续效果（如毒伤、回血）
                        buff.UpdateEffect(deltaTime);
                    }
                }
            }

            // 移除过期Buff的效果
            foreach (var (unitId, buff) in expiredBuffs)
            {
                buff.RemoveEffect();
                EventManager.Instance.Publish(
                    new GameEvents.BuffRemovedEvent(unitId, buff.Data.ID, buff.Data.Name));
            }
        }

        // /// <summary>
        // /// 处理持续效果（DOT/HOT等）
        // /// </summary>
        // private void ProcessContinuousEffect(int unitId, Combat.Buff.Buff buff, float deltaTime)
        // {
        //     var unitData = UnitManager.Instance.Units[unitId];
        //
        //     switch (buff.Data.Type)
        //     {
        //         case BuffType.Poison:
        //             // 持续伤害
        //             var damage = buff.GetEffectValue() * deltaTime;
        //             unitData.CurrentHealth = Mathf.Max(0, unitData.CurrentHealth - damage);
        //             Debug.Log($"单位 {unitId} 受到毒伤害: {damage}，当前血量: {unitData.CurrentHealth}");
        //
        //             // 如果单位死亡，可以发布死亡事件
        //             if (!unitData.IsAlive)
        //             {
        //                 // EventManager.Instance.PublishEvent(new GameEvents.UnitDeathEvent(...));
        //                 Debug.Log($"单位 {unitId} 因中毒死亡");
        //             }
        //
        //             break;
        //         case BuffType.Regeneration:
        //             // 持续治疗
        //             var healing = buff.GetEffectValue() * deltaTime;
        //             unitData.CurrentHealth = Mathf.Min(unitData.MaxHealth, unitData.CurrentHealth + healing);
        //             Debug.Log($"单位 {unitId} 恢复生命: {healing}，当前血量: {unitData.CurrentHealth}");
        //             break;
        //     }
        // }

        /// <summary>
        /// 获取单位的所有活跃Buff
        /// </summary>
        /// <param name="unitId">单位ID</param>
        /// <returns>Buff列表</returns>
        public List<Combat.Buff.BaseBuff> GetUnitBuffs(int unitId)
        {
            if (_unitBuffs.TryGetValue(unitId, out var buffs))
            {
                return buffs.Where(b => b.IsActive).ToList();
            }

            return new List<Combat.Buff.BaseBuff>();
        }

        /// <summary>
        /// 清除单位的所有Buff
        /// </summary>
        /// <param name="unitId">单位ID</param>
        public void ClearUnitBuffs(int unitId)
        {
            if (!_unitBuffs.TryGetValue(unitId, out var buffs))
                return;

            // 移除所有Buff效果
            foreach (var buff in buffs.Where(b => b.IsActive))
            {
                buff.RemoveEffect();
                EventManager.Instance.Publish(
                    new GameEvents.BuffRemovedEvent(unitId, buff.Data.ID, buff.Data.Name));
            }

            buffs.Clear();
        }

        internal void Reset()
        {
            _unitBuffs.Clear();
        }
    }
}