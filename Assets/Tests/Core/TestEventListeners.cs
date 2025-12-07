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
        public int LastUnitId { get; private set; }
        public Vector3 LastDeathPosition { get; private set; }
        public int LastKillerId { get; private set; }
        public bool EnableLogging { get; set; } = true; // 可控制的日志开关

        public override void OnEventReceived(GameEvents.UnitDeathEvent eventData)
        {
            EventReceived = true;
            LastUnitId = eventData.UnitId;
            LastDeathPosition = eventData.DeathPosition;
            LastKillerId = eventData.KillerId;

            // 只在启用日志时输出
            if (EnableLogging)
            {
                Debug.Log($"[Test] Unit {eventData.UnitId} died at {eventData.DeathPosition}");
            }
        }

        public void Reset()
        {
            EventReceived = false;
            LastUnitId = -1;
            LastDeathPosition = Vector3.zero;
            LastKillerId = -1;
        }
    }

    /// <summary>
    /// 高性能测试监听器（无日志输出）
    /// 专门用于性能测试
    /// </summary>
    public class HighPerformanceTestListener : EventListener<GameEvents.UnitDeathEvent>
    {
        public bool EventReceived { get; private set; }
        public int LastUnitId { get; private set; } = -1;
        public int CallCount { get; private set; }

        public override void OnEventReceived(GameEvents.UnitDeathEvent eventData)
        {
            EventReceived = true;
            LastUnitId = eventData.UnitId;
            CallCount++;
            // 无Debug.Log输出，最大化性能
        }

        public void Reset()
        {
            EventReceived = false;
            LastUnitId = -1;
            CallCount = 0;
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
        public bool EnableLogging { get; set; } = true; // 可控制的日志开关

        public override void OnEventReceived(GameEvents.WaveStartEvent eventData)
        {
            EventReceived = true;
            LastWaveNumber = eventData.WaveNumber;
            LastEnemyCount = eventData.EnemyCount;

            // 只在启用日志时输出
            if (EnableLogging)
            {
                Debug.Log($"[Test] Wave {eventData.WaveNumber} started with {eventData.EnemyCount} enemies");
            }
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
        public bool EnableLogging { get; set; } = true; // 可控制的日志开关

        public override void OnEventReceived(GameEvents.PlayerLevelUpEvent eventData)
        {
            EventReceived = true;
            LastNewLevel = eventData.NewLevel;
            LastExperienceGained = eventData.ExperienceGained;

            // 只在启用日志时输出
            if (EnableLogging)
            {
                Debug.Log(
                    $"[Test] Player leveled up to {eventData.NewLevel}! Gained {eventData.ExperienceGained} experience");
            }
        }

        public void Reset()
        {
            EventReceived = false;
            LastNewLevel = 0;
            LastExperienceGained = 0;
        }
    }
}