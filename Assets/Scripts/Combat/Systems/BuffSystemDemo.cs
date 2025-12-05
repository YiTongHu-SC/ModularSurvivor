using Core.Events;
using Core.Units;
using UnityEngine;

namespace Combat.Systems
{
    /// <summary>
    /// BuffSystem使用演示类
    /// 展示如何使用Buff系统的各种功能
    /// </summary>
    public class BuffSystemDemo : MonoBehaviour
    {
        private void Start()
        {
            // 注册事件监听 - 使用委托方式
            EventManager.Instance.Subscribe<GameEvents.BuffAppliedEvent>(OnBuffApplied, this);
            EventManager.Instance.Subscribe<GameEvents.BuffRemovedEvent>(OnBuffRemoved, this);
            
            // 演示Buff系统使用
            DemoBuffSystem();
        }
        
        private void OnDestroy()
        {
            // 取消订阅事件
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<GameEvents.BuffAppliedEvent>(OnBuffApplied, this);
                EventManager.Instance.Unsubscribe<GameEvents.BuffRemovedEvent>(OnBuffRemoved, this);
            }
        }
        
        private void OnBuffApplied(GameEvents.BuffAppliedEvent eventData)
        {
            Debug.Log($"收到Buff应用事件: 单位{eventData.UnitId} 获得了 {eventData.BuffName}");
        }
        
        private void OnBuffRemoved(GameEvents.BuffRemovedEvent eventData)
        {
            Debug.Log($"收到Buff移除事件: 单位{eventData.UnitId} 失去了 {eventData.BuffName}");
        }
        
        private void DemoBuffSystem()
        {
            // 创建一个测试单位
            var unitData = new UnitData(Vector2.zero, 0)
            {
                ID = 1001,
                MoveSpeed = 5f,
                AttackPower = 10f,
                MaxHealth = 100f,
                CurrentHealth = 100f
            };
            
            // 注册到UnitManager
            UnitManager.Instance.UnitSystem.RegisterUnit(unitData);
            
            Debug.Log($"创建单位 {unitData.ID}，初始属性：速度={unitData.MoveSpeed}, 攻击力={unitData.AttackPower}, 血量={unitData.CurrentHealth}");
            
            // 应用速度提升Buff
            CombatManager.Instance.BuffSystem.ApplyBuff(1001, 1);
            Debug.Log($"应用速度提升后：速度={unitData.MoveSpeed}");
            
            // 应用攻击力提升Buff
            CombatManager.Instance.BuffSystem.ApplyBuff(1001, 3);
            Debug.Log($"应用攻击力提升后：攻击力={unitData.AttackPower}");
            
            // 应用中毒Buff
            CombatManager.Instance.BuffSystem.ApplyBuff(1001, 4);
            Debug.Log($"应用中毒效果");
            
            // 查看单位当前所有Buff
            var buffs = CombatManager.Instance.BuffSystem.GetUnitBuffs(1001);
            Debug.Log($"单位当前有 {buffs.Count} 个Buff：");
            foreach (var buff in buffs)
            {
                Debug.Log($"  - {buff.Data.Name} (剩余时间: {buff.RemainingTime:F1}s, 层数: {buff.StackCount})");
            }
        }
        
        // 演示如何在运行时添加更多Buff
        [ContextMenu("应用额外速度提升")]
        public void ApplyExtraSpeedBoost()
        {
            CombatManager.Instance.BuffSystem.ApplyBuff(1001, 1); // 叠加速度提升
        }
        
        [ContextMenu("移除中毒效果")]
        public void RemovePoison()
        {
            CombatManager.Instance.BuffSystem.RemoveBuff(1001, 4); // 移除中毒
        }
        
        [ContextMenu("清除所有Buff")]
        public void ClearAllBuffs()
        {
            CombatManager.Instance.BuffSystem.ClearUnitBuffs(1001);
        }
    }
}
