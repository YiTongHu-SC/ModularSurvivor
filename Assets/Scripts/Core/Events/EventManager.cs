using System;
using System.Collections.Generic;
using StellarCore.Singleton;

namespace Core.Events
{
    /// <summary>
    ///     事件管理器（单例）
    ///     提供全局事件系统，实现模块间的松耦合通信
    ///     支持监听器对象和委托两种订阅方式
    ///     使用延迟移除机制解决发布过程中取消订阅的并发修改问题
    /// </summary>
    public class EventManager : BaseInstance<EventManager>
    {
        // 事件监听器字典，按事件类型分组
        private readonly Dictionary<Type, List<IEventListener<EventData>>> _eventListeners = new();

        // 委托订阅字典，按事件类型分组，存储委托适配器
        private readonly Dictionary<Type, List<object>> _eventSubscribers = new();

        // 事件监听器字典，按监听器实例分组（用于快速注销）
        private readonly Dictionary<IEventListener<EventData>, List<Type>> _listenerEvents = new();

        // 待移除的监听器队列（延迟移除机制）
        private readonly List<(Type eventType, IEventListener<EventData> listener)> _pendingListenerRemovals = new();

        // 待移除的委托适配器队列（延迟移除机制）
        private readonly List<(Type eventType, object adapter)> _pendingSubscriberRemovals = new();

        // 发布状态控制
        private bool _isPublishing;

        protected void OnDestroy()
        {
            ClearAllListeners();
        }

        /// <summary>
        ///     处理待移除的监听器和委托（延迟移除机制的核心）
        /// </summary>
        private void ProcessPendingRemovals()
        {
            // 处理待移除的监听器
            foreach (var (eventType, listener) in _pendingListenerRemovals)
            {
                if (_eventListeners.TryGetValue(eventType, out var listeners))
                {
                    listeners.Remove(listener);
                    if (listeners.Count == 0)
                        _eventListeners.Remove(eventType);
                }

                if (_listenerEvents.TryGetValue(listener, out var eventTypes))
                {
                    eventTypes.Remove(eventType);
                    if (eventTypes.Count == 0)
                        _listenerEvents.Remove(listener);
                }
            }

            // 处理待移除的委托适配器
            foreach (var (eventType, pendingAdapter) in _pendingSubscriberRemovals)
            {
                if (_eventSubscribers.TryGetValue(eventType, out var subscribers))
                {
                    subscribers.Remove(pendingAdapter);
                    if (subscribers.Count == 0)
                        _eventSubscribers.Remove(eventType);
                }
            }

            // 清空待移除队列
            _pendingListenerRemovals.Clear();
            _pendingSubscriberRemovals.Clear();
        }

        /// <summary>
        ///     发布事件
        /// </summary>
        /// <param name="eventData">事件数据</param>
        public void Publish(EventData eventData)
        {
            if (eventData == null) return;

            var eventType = eventData.GetType();

            // 设置发布状态，防止在发布过程中直接修改监听器列表
            _isPublishing = true;

            try
            {
                // 触发监听器对象
                if (_eventListeners.TryGetValue(eventType, out var listeners) && listeners.Count > 0)
                {
                    foreach (var t in listeners)
                    {
                        t.OnEventReceived(eventData);
                    }
                }

                // 触发委托订阅者
                if (_eventSubscribers.TryGetValue(eventType, out var subscribers) && subscribers.Count > 0)
                {
                    foreach (var adapter in subscribers)
                    {
                        if (adapter is ICallbackAdapter callbackAdapter)
                        {
                            callbackAdapter.Invoke(eventData);
                        }
                    }
                }
            }
            finally
            {
                // 重置发布状态
                _isPublishing = false;

                // 处理所有待移除的项目
                if (_pendingListenerRemovals.Count > 0 || _pendingSubscriberRemovals.Count > 0)
                    ProcessPendingRemovals();
            }
        }

        /// <summary>
        ///     订阅事件
        /// </summary>
        /// <param name="listener">事件监听器</param>
        public void Subscribe<T>(IEventListener<T> listener) where T : EventData
        {
            if (listener == null) return;

            var eventType = typeof(T);

            // 创建适配器包装器来处理类型转换
            IEventListener<EventData> adaptedListener = new EventListenerAdapter<T>(listener);

            // 添加到事件类型字典
            if (!_eventListeners.ContainsKey(eventType))
                _eventListeners[eventType] = new List<IEventListener<EventData>>();

            if (!_eventListeners[eventType].Contains(adaptedListener))
                _eventListeners[eventType].Add(adaptedListener);

            // 添加到监听器字典
            if (!_listenerEvents.ContainsKey(adaptedListener))
                _listenerEvents[adaptedListener] = new List<Type>();

            if (!_listenerEvents[adaptedListener].Contains(eventType))
                _listenerEvents[adaptedListener].Add(eventType);
        }

        /// <summary>
        ///     取消订阅事件
        /// </summary>
        /// <param name="listener">事件监听器</param>
        public void Unsubscribe<T>(IEventListener<T> listener) where T : EventData
        {
            if (listener == null) return;

            var eventType = typeof(T);

            // 创建适配器包装器来处理类型转换
            IEventListener<EventData> adaptedListener = new EventListenerAdapter<T>(listener);

            // 如果正在发布，将移除操作加入待处理队列
            if (_isPublishing)
            {
                _pendingListenerRemovals.Add((eventType, adaptedListener));
                return;
            }

            // 立即移除
            if (_eventListeners.TryGetValue(eventType, out var listeners))
            {
                listeners.Remove(adaptedListener);
                if (listeners.Count == 0)
                    _eventListeners.Remove(eventType);
            }

            if (_listenerEvents.TryGetValue(adaptedListener, out var eventTypes))
            {
                eventTypes.Remove(eventType);
                if (eventTypes.Count == 0)
                    _listenerEvents.Remove(adaptedListener);
            }
        }

        /// <summary>
        ///     获取指定事件类型的监听器数量（包括监听器对象和委托订阅者）
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <returns>总监听器数量</returns>
        public int GetListenerCount(Type eventType)
        {
            var listenerCount = _eventListeners.ContainsKey(eventType) ? _eventListeners[eventType].Count : 0;
            var subscriberCount = _eventSubscribers.ContainsKey(eventType) ? _eventSubscribers[eventType].Count : 0;
            return listenerCount + subscriberCount;
        }

        /// <summary>
        ///     获取指定事件类型的监听器对象数量
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <returns>监听器对象数量</returns>
        public int GetListenerObjectCount(Type eventType)
        {
            return _eventListeners.ContainsKey(eventType) ? _eventListeners[eventType].Count : 0;
        }

        /// <summary>
        ///     获取指定事件类型的委托订阅者数量
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <returns>委托订阅者数量</returns>
        public int GetSubscriberCount(Type eventType)
        {
            return _eventSubscribers.ContainsKey(eventType) ? _eventSubscribers[eventType].Count : 0;
        }

        /// <summary>
        ///     清除所有事件监听器（包括监听器对象和委托订阅者）
        /// </summary>
        public void ClearAllListeners()
        {
            _eventListeners.Clear();
            _listenerEvents.Clear();
            _eventSubscribers.Clear();
            _pendingListenerRemovals.Clear();
            _pendingSubscriberRemovals.Clear();
        }

        /// <summary>
        ///     事件监听器适配器，用于处理泛型类型转换
        /// </summary>
        /// <typeparam name="T">具体事件类型</typeparam>
        private class EventListenerAdapter<T> : IEventListener<EventData> where T : EventData
        {
            private readonly IEventListener<T> _originalListener;

            public EventListenerAdapter(IEventListener<T> originalListener)
            {
                _originalListener = originalListener;
            }

            public void OnEventReceived(EventData eventData)
            {
                if (eventData is T specificEventData) _originalListener.OnEventReceived(specificEventData);
            }

            // 重写 Equals 和 GetHashCode 以支持正确的比较
            public override bool Equals(object obj)
            {
                return obj is EventListenerAdapter<T> adapter &&
                       ReferenceEquals(_originalListener, adapter._originalListener);
            }

            public override int GetHashCode()
            {
                return _originalListener?.GetHashCode() ?? 0;
            }
        }

        /// <summary>
        ///     委托适配器接口
        /// </summary>
        private interface ICallbackAdapter
        {
            void Invoke(EventData eventData);
        }

        /// <summary>
        ///     委托适配器，用于处理委托订阅的泛型类型转换和匹配
        /// </summary>
        /// <typeparam name="T">具体事件类型</typeparam>
        private class CallbackAdapter<T> : ICallbackAdapter where T : EventData
        {
            private readonly Action<T> _originalCallback;

            public CallbackAdapter(Action<T> originalCallback)
            {
                _originalCallback = originalCallback;
            }

            public void Invoke(EventData eventData)
            {
                if (eventData is T specificEventData)
                    _originalCallback(specificEventData);
            }

            // 重写 Equals 和 GetHashCode 以支持正确的比较
            public override bool Equals(object obj)
            {
                return obj is CallbackAdapter<T> adapter &&
                       Delegate.Equals(_originalCallback, adapter._originalCallback);
            }

            public override int GetHashCode()
            {
                return _originalCallback?.GetHashCode() ?? 0;
            }
        }

        #region 委托订阅方法

        /// <summary>
        ///     委托方式订阅事件
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="callback">回调函数</param>
        public void Subscribe<T>(Action<T> callback) where T : EventData
        {
            if (callback == null) return;

            var eventType = typeof(T);

            // 创建委托适配器
            var adapter = new CallbackAdapter<T>(callback);

            // 添加到委托订阅字典
            if (!_eventSubscribers.ContainsKey(eventType))
                _eventSubscribers[eventType] = new List<object>();

            _eventSubscribers[eventType].Add(adapter);
        }

        /// <summary>
        ///     取消委托订阅
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="callback">原始回调函数</param>
        public void Unsubscribe<T>(Action<T> callback) where T : EventData
        {
            if (callback == null) return;

            var eventType = typeof(T);
            var searchAdapter = new CallbackAdapter<T>(callback);

            // 如果正在发布，创建适配器并加入待处理队列
            if (_isPublishing)
            {
                _pendingSubscriberRemovals.Add((eventType, searchAdapter));
                return;
            }

            // 立即移除
            if (_eventSubscribers.TryGetValue(eventType, out var subscribers))
            {
                subscribers.Remove(searchAdapter);
                if (subscribers.Count == 0)
                    _eventSubscribers.Remove(eventType);
            }
        }

        #endregion
    }
}