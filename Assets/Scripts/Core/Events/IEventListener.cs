namespace Core.Events
{
    /// <summary>
    /// 事件监听器接口
    /// </summary>
    public interface IEventListener
    {
        /// <summary>
        /// 处理事件
        /// </summary>
        /// <param name="eventData">事件数据</param>
        void OnEventReceived(EventData eventData);
        
        /// <summary>
        /// 获取监听的事件类型
        /// </summary>
        /// <returns>事件类型</returns>
        System.Type GetEventType();
    }
}

