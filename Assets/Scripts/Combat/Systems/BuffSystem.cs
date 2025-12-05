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
        private readonly Dictionary<int, List<Combat.Buff.Buff>> _unitBuffs = new();
        
        // 预定义的Buff模板
        private readonly Dictionary<int, BuffData> _buffTemplates = new();
        
        public void Initialize()
        {
            InitializeBuffTemplates();
        }
        
        /// <summary>
        /// 初始化Buff模板数据
        /// </summary>
        private void InitializeBuffTemplates()
        {
            // 预定义一些常用的Buff
            _buffTemplates[1] = new BuffData(1, "速度提升", BuffType.SpeedBoost, 5f, 2f, true);
            _buffTemplates[2] = new BuffData(2, "速度减缓", BuffType.SpeedReduction, 3f, 0.5f);
            _buffTemplates[3] = new BuffData(3, "攻击提升", BuffType.AttackBoost, 10f, 1.5f, true);
            _buffTemplates[4] = new BuffData(4, "中毒", BuffType.Poison, 8f, 10f);
            _buffTemplates[5] = new BuffData(5, "生命恢复", BuffType.Regeneration, 15f, 5f);
        }
        
        /// <summary>
        /// 应用Buff到指定单位
        /// </summary>
        /// <param name="unitId">目标单位ID</param>
        /// <param name="buffId">Buff模板ID</param>
        /// <returns>是否成功应用</returns>
        public bool ApplyBuff(int unitId, int buffId)
        {
            if (!_buffTemplates.TryGetValue(buffId, out var buffTemplate))
            {
                Debug.LogWarning($"未找到Buff模板: {buffId}");
                return false;
            }
            
            // 检查单位是否存在
            if (!UnitManager.Instance.Units.ContainsKey(unitId))
            {
                Debug.LogWarning($"未找到单位: {unitId}");
                return false;
            }
            
            // 确保单位有Buff列表
            if (!_unitBuffs.ContainsKey(unitId))
            {
                _unitBuffs[unitId] = new List<Combat.Buff.Buff>();
            }
            
            var newBuff = new Combat.Buff.Buff(buffTemplate, unitId);
            var unitBuffs = _unitBuffs[unitId];
            
            // 尝试叠加现有Buff
            var existingBuff = unitBuffs.FirstOrDefault(b => b.Data.ID == buffId && b.IsActive);
            if (existingBuff != null && existingBuff.TryStack(newBuff))
            {
                Debug.Log($"Buff叠加成功: {buffTemplate.Name} (层数: {existingBuff.StackCount})");
            }
            else
            {
                // 添加新Buff
                unitBuffs.Add(newBuff);
                Debug.Log($"应用新Buff: {buffTemplate.Name} 到单位 {unitId}");
            }
            
            // 应用Buff效果
            ApplyBuffEffect(unitId, newBuff);
            
            // 发布事件
            EventManager.Instance.PublishEvent(new GameEvents.BuffAppliedEvent(unitId, buffId, buffTemplate.Name));
            
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
            RemoveBuffEffect(unitId, buffToRemove);
            
            // 标记为非活跃
            buffToRemove.IsActive = false;
            
            // 发布事件
            EventManager.Instance.PublishEvent(new GameEvents.BuffRemovedEvent(unitId, buffId, buffToRemove.Data.Name));
            
            return true;
        }
        
        /// <summary>
        /// 更新所有Buff（每帧调用）
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        public void UpdateBuffs(float deltaTime)
        {
            var expiredBuffs = new List<(int unitId, Combat.Buff.Buff buff)>();
            
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
                        ProcessContinuousEffect(unitId, buff, deltaTime);
                    }
                }
            }
            
            // 移除过期Buff的效果
            foreach (var (unitId, buff) in expiredBuffs)
            {
                RemoveBuffEffect(unitId, buff);
                EventManager.Instance.PublishEvent(new GameEvents.BuffRemovedEvent(unitId, buff.Data.ID, buff.Data.Name));
            }
        }
        
        /// <summary>
        /// 应用Buff效果到单位属性
        /// </summary>
        private void ApplyBuffEffect(int unitId, Combat.Buff.Buff buff)
        {
            var unitData = UnitManager.Instance.Units[unitId];
            
            switch (buff.Data.Type)
            {
                case BuffType.SpeedBoost:
                    unitData.MoveSpeed *= buff.GetEffectValue();
                    break;
                case BuffType.SpeedReduction:
                    unitData.MoveSpeed *= buff.GetEffectValue();
                    break;
                case BuffType.AttackBoost:
                    unitData.AttackPower *= buff.GetEffectValue();
                    break;
                case BuffType.AttackReduction:
                    unitData.AttackPower *= buff.GetEffectValue();
                    break;
                // DOT/HOT类型的Buff不需要在这里处理，由ProcessContinuousEffect处理
            }
        }
        
        /// <summary>
        /// 移除Buff效果
        /// </summary>
        private void RemoveBuffEffect(int unitId, Combat.Buff.Buff buff)
        {
            var unitData = UnitManager.Instance.Units[unitId];
            
            switch (buff.Data.Type)
            {
                case BuffType.SpeedBoost:
                    unitData.MoveSpeed /= buff.GetEffectValue();
                    break;
                case BuffType.SpeedReduction:
                    unitData.MoveSpeed /= buff.GetEffectValue();
                    break;
                case BuffType.AttackBoost:
                    unitData.AttackPower /= buff.GetEffectValue();
                    break;
                case BuffType.AttackReduction:
                    unitData.AttackPower /= buff.GetEffectValue();
                    break;
                // DOT/HOT类型的Buff不需要恢复，因为它们的效果是瞬时的
            }
        }
        
        /// <summary>
        /// 处理持续效果（DOT/HOT等）
        /// </summary>
        private void ProcessContinuousEffect(int unitId, Combat.Buff.Buff buff, float deltaTime)
        {
            var unitData = UnitManager.Instance.Units[unitId];
            
            switch (buff.Data.Type)
            {
                case BuffType.Poison:
                    // 持续伤害
                    var damage = buff.GetEffectValue() * deltaTime;
                    unitData.CurrentHealth = Mathf.Max(0, unitData.CurrentHealth - damage);
                    Debug.Log($"单位 {unitId} 受到毒伤害: {damage}，当前血量: {unitData.CurrentHealth}");
                    
                    // 如果单位死亡，可以发布死亡事件
                    if (!unitData.IsAlive)
                    {
                        // EventManager.Instance.PublishEvent(new GameEvents.UnitDeathEvent(...));
                        Debug.Log($"单位 {unitId} 因中毒死亡");
                    }
                    break;
                case BuffType.Regeneration:
                    // 持续治疗
                    var healing = buff.GetEffectValue() * deltaTime;
                    unitData.CurrentHealth = Mathf.Min(unitData.MaxHealth, unitData.CurrentHealth + healing);
                    Debug.Log($"单位 {unitId} 恢复生命: {healing}，当前血量: {unitData.CurrentHealth}");
                    break;
            }
        }
        
        /// <summary>
        /// 获取单位的所有活跃Buff
        /// </summary>
        /// <param name="unitId">单位ID</param>
        /// <returns>Buff列表</returns>
        public List<Combat.Buff.Buff> GetUnitBuffs(int unitId)
        {
            if (_unitBuffs.TryGetValue(unitId, out var buffs))
            {
                return buffs.Where(b => b.IsActive).ToList();
            }
            return new List<Combat.Buff.Buff>();
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
                RemoveBuffEffect(unitId, buff);
                EventManager.Instance.PublishEvent(new GameEvents.BuffRemovedEvent(unitId, buff.Data.ID, buff.Data.Name));
            }
            
            buffs.Clear();
        }
        
        /// <summary>
        /// 获取Buff模板
        /// </summary>
        /// <param name="buffId">Buff ID</param>
        /// <returns>Buff模板数据</returns>
        public BuffData GetBuffTemplate(int buffId)
        {
            _buffTemplates.TryGetValue(buffId, out var template);
            return template;
        }
    }
}