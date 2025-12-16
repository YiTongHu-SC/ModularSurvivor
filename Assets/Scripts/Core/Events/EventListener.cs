using System;

namespace Core.Events
{
    /// <summary>
    ///     泛型事件监听器基类
    ///     简化具体监听器的实现
    /// </summary>
    /// <typeparam name="T">监听的事件类型</typeparam>
    public abstract class EventListener<T> : IEventListener<T> where T : EventData
    {
        /// <summary>
        ///     处理具体类型的事件（泛型接口实现）
        /// </summary>
        /// <param name="eventData">具体类型的事件数据</param>
        public abstract void OnEventReceived(T eventData);

        /// <summary>
        ///     获取监听的事件类型
        /// </summary>
        /// <returns>事件类型</returns>
        public Type GetEventType()
        {
            return typeof(T);
        }

        /// <summary>
        ///     处理事件（非泛型接口实现）
        /// </summary>
        /// <param name="eventData">事件数据</param>
        public void OnEventReceived(EventData eventData)
        {
            if (eventData is T specificEventData) OnEventReceived(specificEventData);
        }
    }
}