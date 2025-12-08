using Core.Units;

namespace Combat.Buff
{
    /// <summary>
    /// Buff实例类，管理单个Buff的状态和生命周期
    /// </summary>
    public abstract class BaseBuff
    {
        public UnitData TargetUnitData => UnitManager.Instance.Units[TargetUnitId];
        public BuffData Data { get; private set; }
        public int TargetUnitId { get; private set; }
        public float RemainingTime { get; private set; }
        public int StackCount { get; private set; }
        public bool IsActive { get; set; }

        protected BaseBuff(BuffData data, int targetUnitId)
        {
            Data = data;
            TargetUnitId = targetUnitId;
            RemainingTime = data.Duration;
            StackCount = 1;
            IsActive = true;
        }

        /// <summary>
        /// 更新Buff时间
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        /// <returns>是否仍然有效</returns>
        public virtual bool UpdateTime(float deltaTime)
        {
            if (!IsActive) return false;

            RemainingTime -= deltaTime;
            if (RemainingTime <= 0)
            {
                IsActive = false;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 尝试叠加Buff
        /// </summary>
        /// <param name="newBaseBuff">新的Buff</param>
        /// <returns>是否成功叠加</returns>
        public virtual bool TryStack(BaseBuff newBaseBuff)
        {
            if (!Data.CanStack || Data.ID != newBaseBuff.Data.ID) return false;

            StackCount++;
            // 刷新持续时间
            RemainingTime = Data.Duration;
            return true;
        }

        /// <summary>
        /// 获取当前效果值（考虑叠加）
        /// </summary>
        public virtual float GetEffectValue()
        {
            return Data.Value * StackCount;
        }

        /// <summary>
        /// 应用Buff效果到单位属性,
        /// DOT/HOT类型的Buff不需要在这里处理，由UpdateEffect处理
        /// </summary>
        public abstract void ApplyEffect();

        /// <summary>
        /// 移除Buff效果,
        /// </summary>
        public abstract void RemoveEffect();

        /// <summary>
        /// 更新Buff效果（每帧调用）DOT/HOT类的Buff
        /// </summary>
        /// <param name="deltaTime"></param>
        public abstract void UpdateEffect(float deltaTime);
    }
}