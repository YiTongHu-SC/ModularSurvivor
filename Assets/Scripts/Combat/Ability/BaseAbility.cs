using Core.Units;

namespace Combat.Ability
{
    public abstract class BaseAbility
    {
        public int TargetUnitId { get; private set; }
        public UnitData TargetUnitData => UnitManager.Instance.Units[TargetUnitId];
        public AbilityData Data { get; private set; }
        public bool IsActive { get; set; }

        protected BaseAbility(AbilityData data, int targetId)
        {
            Data = data;
            TargetUnitId = targetId;
            IsActive = true;
        }

        /// <summary>
        /// 应用能力效果，新增能力时调用一次
        /// </summary>
        public abstract void ApplyAbility();

        /// <summary>
        /// 移除能力效果，能力结束时调用一次
        /// </summary>
        public abstract void RemoveAbility();

        /// <summary>
        /// 更新能力状态，每帧调用
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        public abstract void UpdateAbility(float deltaTime);

        /// <summary>
        /// 执行能力的主要效果
        /// </summary>
        public abstract void PerformAbility();
    }
}