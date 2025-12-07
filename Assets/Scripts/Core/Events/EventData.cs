using System;

namespace Core.Events
{
    /// <summary>
    /// 事件数据基类
    /// </summary>
    public abstract class EventData
    {
        /// <summary>
        /// 事件发生时间
        /// </summary>
        public DateTime Timestamp { get; private set; } = DateTime.Now;
    }
}
