using Core.Units;

namespace Core.Events
{
    /// <summary>
    ///     游戏事件类型定义
    /// </summary>
    public static class GameEvents
    {
        public class GameInitializedEvent : EventData
        {
        }

        public class UnitMovementEvent : EventData
        {
            public UnitMovementEvent(UnitData unitData)
            {
                GUID = unitData.GUID;
                UnitData = unitData;
            }

            public int GUID { get; }
            public UnitData UnitData { get; set; }
        }

        /// <summary>
        ///     单位死亡事件
        /// </summary>
        public class UnitDeathEvent : EventData
        {
            public UnitDeathEvent(int guid, int killerId = -1)
            {
                GUID = guid;
                KillerId = killerId;
            }

            public int GUID { get; }
            public int KillerId { get; }
        }

        /// <summary>
        ///     波次开始事件
        /// </summary>
        public class WaveStartEvent : EventData
        {
            public WaveStartEvent(int waveNumber, int enemyCount)
            {
                WaveNumber = waveNumber;
                EnemyCount = enemyCount;
            }

            public int WaveNumber { get; }
            public int EnemyCount { get; }
        }

        /// <summary>
        ///     波次结束事件
        /// </summary>
        public class WaveEndEvent : EventData
        {
            public WaveEndEvent(int waveNumber, bool isSuccess, float duration)
            {
                WaveNumber = waveNumber;
                IsSuccess = isSuccess;
                Duration = duration;
            }

            public int WaveNumber { get; }
            public bool IsSuccess { get; }
            public float Duration { get; }
        }

        /// <summary>
        ///     玩家升级事件
        /// </summary>
        public class PlayerLevelUpEvent : EventData
        {
            public PlayerLevelUpEvent(int newLevel, int experienceGained)
            {
                NewLevel = newLevel;
                ExperienceGained = experienceGained;
            }

            public int NewLevel { get; }
            public int ExperienceGained { get; }
        }

        /// <summary>
        ///     UI刷新事件
        /// </summary>
        public class UIRefreshEvent : EventData
        {
            public UIRefreshEvent(string uiName, object data = null)
            {
                UIName = uiName;
                Data = data;
            }

            public string UIName { get; }
            public object Data { get; }
        }

        /// <summary>
        ///     Buff应用事件
        /// </summary>
        public class BuffAppliedEvent : EventData
        {
            public BuffAppliedEvent(int buffId, int unitId)
            {
                UnitId = unitId;
                BuffId = buffId;
            }

            public int UnitId { get; }
            public int BuffId { get; }
        }

        /// <summary>
        ///     Buff移除事件
        /// </summary>
        public class BuffRemovedEvent : EventData
        {
            public BuffRemovedEvent(int unitId, int buffId, string buffName)
            {
                UnitId = unitId;
                BuffId = buffId;
                BuffName = buffName;
            }

            public int UnitId { get; }
            public int BuffId { get; }
            public string BuffName { get; }
        }

        /// <summary>
        ///     能力移除事件
        /// </summary>
        public class AbilityRemovedEvent : EventData
        {
            public AbilityRemovedEvent(int unitId, int abilityId)
            {
                UnitId = unitId;
                AbilityId = abilityId;
            }

            public int UnitId { get; }
            public int AbilityId { get; }
        }

        public class OverlapEvent : EventData
        {
            public OverlapEvent(int unitAGuid, int unitBGuid)
            {
                UnitAGuid = unitAGuid;
                UnitBGuid = unitBGuid;
            }

            public int UnitAGuid { get; }
            public int UnitBGuid { get; }
        }

        public class HeroCreated : EventData
        {
            public HeroCreated(int heroDataGuid)
            {
                UnitGuid = heroDataGuid;
            }

            public int UnitGuid { get; }
        }

        public class UpdatePreferenceEvent : EventData
        {
            public UpdatePreferenceEvent(int preferenceId, ViewBaseData view)
            {
                PreferenceId = preferenceId;
                ViewData = view;
            }

            public int PreferenceId { get; }
            public ViewBaseData ViewData { get; }
        }

        public class GameStartEvent : EventData
        {
            public GameStartEvent(int levelId)
            {
                LevelID = levelId;
            }

            public int LevelID { get; }
        }

        public class GameExitEvent : EventData
        {
        }

        public class ReturnToMainMenuEvent : EventData
        {
        }
    }
}