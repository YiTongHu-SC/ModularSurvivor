namespace Core.Events
{
    /// <summary>
    /// 事件监听器接口基类
    /// </summary>
    public interface IEventListener<in T> where T : EventData
    {
        /// <summary>
        /// 处理事件
        /// </summary>
        /// <param name="eventData">事件数据</param>
        void OnEventReceived(T eventData);
    }

    // /// <summary>
    // /// 非泛型事件监听器接口，用于类型擦除
    // /// </summary>
    // public interface IEventListener
    // {
    //     /// <summary>
    //     /// 处理事件（非泛型版本）
    //     /// </summary>
    //     /// <param name="eventData">事件数据</param>
    //     void OnEventReceived(EventData eventData);
    // }
}