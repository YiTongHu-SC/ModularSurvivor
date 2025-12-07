using Core.Units;

namespace Core.Events
{
    /// <summary>
    /// 游戏事件类型定义
    /// </summary>
    public static class GameEvents
    {
        public class UnitMovementEvent : EventData
        {
            public int UnitId { get; }
            public UnitData UnitData { get; set; }

            public UnitMovementEvent(UnitData unitData)
            {
                UnitId = unitData.ID;
                UnitData = unitData;
            }
        }

        /// <summary>
        /// 单位死亡事件
        /// </summary>
        public class UnitDeathEvent : EventData
        {
            public int UnitId { get; }
            public int KillerId { get; }
            public UnityEngine.Vector3 DeathPosition { get; }

            public UnitDeathEvent(int unitId, UnityEngine.Vector3 deathPosition, int killerId = -1)
            {
                UnitId = unitId;
                DeathPosition = deathPosition;
                KillerId = killerId;
            }
        }

        /// <summary>
        /// 波次开始事件
        /// </summary>
        public class WaveStartEvent : EventData
        {
            public int WaveNumber { get; }
            public int EnemyCount { get; }

            public WaveStartEvent(int waveNumber, int enemyCount)
            {
                WaveNumber = waveNumber;
                EnemyCount = enemyCount;
            }
        }

        /// <summary>
        /// 波次结束事件
        /// </summary>
        public class WaveEndEvent : EventData
        {
            public int WaveNumber { get; }
            public bool IsSuccess { get; }
            public float Duration { get; }

            public WaveEndEvent(int waveNumber, bool isSuccess, float duration)
            {
                WaveNumber = waveNumber;
                IsSuccess = isSuccess;
                Duration = duration;
            }
        }

        /// <summary>
        /// 玩家升级事件
        /// </summary>
        public class PlayerLevelUpEvent : EventData
        {
            public int NewLevel { get; }
            public int ExperienceGained { get; }

            public PlayerLevelUpEvent(int newLevel, int experienceGained)
            {
                NewLevel = newLevel;
                ExperienceGained = experienceGained;
            }
        }

        /// <summary>
        /// UI刷新事件
        /// </summary>
        public class UIRefreshEvent : EventData
        {
            public string UIName { get; }
            public object Data { get; }

            public UIRefreshEvent(string uiName, object data = null)
            {
                UIName = uiName;
                Data = data;
            }
        }

        /// <summary>
        /// Buff应用事件
        /// </summary>
        public class BuffAppliedEvent : EventData
        {
            public int UnitId { get; }
            public int BuffId { get; }

            public BuffAppliedEvent(int buffId, int unitId)
            {
                UnitId = unitId;
                BuffId = buffId;
            }
        }

        /// <summary>
        /// Buff移除事件
        /// </summary>
        public class BuffRemovedEvent : EventData
        {
            public int UnitId { get; }
            public int BuffId { get; }
            public string BuffName { get; }

            public BuffRemovedEvent(int unitId, int buffId, string buffName)
            {
                UnitId = unitId;
                BuffId = buffId;
                BuffName = buffName;
            }
        }
    }
}