using Core.Events;
using UnityEngine;

namespace Tests.Core.Events
{
    /// <summary>
    /// 测试用的单位死亡事件监听器
    /// </summary>
    public class TestUnitDeathListener : EventListener<GameEvents.UnitDeathEvent>
    {
        public bool EventReceived { get; private set; }
        public string LastUnitId { get; private set; }
        public Vector3 LastDeathPosition { get; private set; }
        public string LastKillerId { get; private set; }

        protected override void OnEventReceived(GameEvents.UnitDeathEvent eventData)
        {
            EventReceived = true;
            LastUnitId = eventData.UnitId;
            LastDeathPosition = eventData.DeathPosition;
            LastKillerId = eventData.KillerId;
            
            Debug.Log($"[Test] Unit {eventData.UnitId} died at {eventData.DeathPosition}");
        }

        public void Reset()
        {
            EventReceived = false;
            LastUnitId = null;
            LastDeathPosition = Vector3.zero;
            LastKillerId = null;
        }
    }
    
    /// <summary>
    /// 测试用的波次开始事件监听器
    /// </summary>
    public class TestWaveStartListener : EventListener<GameEvents.WaveStartEvent>
    {
        public bool EventReceived { get; private set; }
        public int LastWaveNumber { get; private set; }
        public int LastEnemyCount { get; private set; }

        protected override void OnEventReceived(GameEvents.WaveStartEvent eventData)
        {
            EventReceived = true;
            LastWaveNumber = eventData.WaveNumber;
            LastEnemyCount = eventData.EnemyCount;
            
            Debug.Log($"[Test] Wave {eventData.WaveNumber} started with {eventData.EnemyCount} enemies");
        }

        public void Reset()
        {
            EventReceived = false;
            LastWaveNumber = 0;
            LastEnemyCount = 0;
        }
    }
    
    /// <summary>
    /// 测试用的玩家升级事件监听器
    /// </summary>
    public class TestPlayerLevelUpListener : EventListener<GameEvents.PlayerLevelUpEvent>
    {
        public bool EventReceived { get; private set; }
        public int LastNewLevel { get; private set; }
        public int LastExperienceGained { get; private set; }

        protected override void OnEventReceived(GameEvents.PlayerLevelUpEvent eventData)
        {
            EventReceived = true;
            LastNewLevel = eventData.NewLevel;
            LastExperienceGained = eventData.ExperienceGained;
            
            Debug.Log($"[Test] Player leveled up to {eventData.NewLevel}! Gained {eventData.ExperienceGained} experience");
        }

        public void Reset()
        {
            EventReceived = false;
            LastNewLevel = 0;
            LastExperienceGained = 0;
        }
    }
}
