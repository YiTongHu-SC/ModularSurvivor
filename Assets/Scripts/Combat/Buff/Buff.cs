
namespace Combat.Buff
{
    /// <summary>
    /// Buff实例类，管理单个Buff的状态和生命周期
    /// </summary>
    public class Buff
    {
        public BuffData Data { get; private set; }
        public int TargetUnitId { get; private set; }
        public float RemainingTime { get; private set; }
        public int StackCount { get; private set; }
        public bool IsActive { get; set; }
        
        public Buff(BuffData data, int targetUnitId)
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
        public bool UpdateTime(float deltaTime)
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
        /// <param name="newBuff">新的Buff</param>
        /// <returns>是否成功叠加</returns>
        public bool TryStack(Buff newBuff)
        {
            if (!Data.CanStack || Data.ID != newBuff.Data.ID) return false;
            
            StackCount++;
            // 刷新持续时间
            RemainingTime = Data.Duration;
            return true;
        }
        
        /// <summary>
        /// 获取当前效果值（考虑叠加）
        /// </summary>
        public float GetEffectValue()
        {
            return Data.Value * StackCount;
        }
    }
}