using Core.Abstructs;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Events
{
    /// <summary>
    /// 事件管理器（单例）
    /// 提供全局事件系统，实现模块间的松耦合通信
    /// 支持监听器对象和委托两种订阅方式
    /// </summary>
    public class EventManager : BaseInstance<EventManager>
    {
        // 事件监听器字典，按事件类型分组
        private readonly Dictionary<Type, List<IEventListener<EventData>>> _eventListeners = new();

        // 事件监听器字典，按监听器实例分组（用于快速注销）
        private readonly Dictionary<IEventListener<EventData>, List<Type>> _listenerEvents = new();

        // 委托订阅字典，按事件类型分组
        private readonly Dictionary<Type, List<Action<EventData>>> _eventSubscribers = new();

        // 委托订阅管理，按拥有者分组（用于批量清理）
        private readonly Dictionary<object, List<(Type eventType, Action<EventData> callback)>> _subscriberOwners =
            new();

        // 性能开关：是否启用详细日志（生产环境可关闭）
        private bool _enableVerboseLogging = false;

        // 超高性能模式：连异常日志都关闭（仅限性能测试）
        private bool _enableHighPerformanceMode = false;

        public override void Initialize()
        {
            Debug.Log("EventManager initialized.");
        }

        /// <summary>
        /// 设置详细日志开关（生产环境建议关闭以提高性能）
        /// </summary>
        /// <param name="enabled">是否启用详细日志</param>
        public void SetVerboseLogging(bool enabled)
        {
            _enableVerboseLogging = enabled;
            Debug.Log($"EventManager: Verbose logging {(enabled ? "enabled" : "disabled")}.");
        }

        /// <summary>
        /// 设置高性能模式（性能测试时可启用，会关闭所有日志包括异常日志）
        /// </summary>
        /// <param name="enabled">是否启用高性能模式</param>
        public void SetHighPerformanceMode(bool enabled)
        {
            _enableHighPerformanceMode = enabled;
            if (enabled)
            {
                _enableVerboseLogging = false; // 高性能模式下自动关闭详细日志
            }

            Debug.Log($"EventManager: High performance mode {(enabled ? "enabled" : "disabled")}.");
        }

        protected override void Awake()
        {
            base.Awake();
            if (Instance == this)
            {
                Initialize();
            }
        }

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="eventData">事件数据</param>
        public void PublishEvent(EventData eventData)
        {
            if (eventData == null)
            {
                if (_enableVerboseLogging)
                    Debug.LogWarning("EventManager: Trying to publish null event data.");
                return;
            }

            Type eventType = eventData.GetType();

            // 高性能模式：极简代码路径
            if (_enableHighPerformanceMode)
            {
                // 触发监听器对象 - 无异常处理和日志
                if (_eventListeners.TryGetValue(eventType, out List<IEventListener<EventData>> listeners) &&
                    listeners.Count > 0)
                {
                    for (int i = 0; i < listeners.Count; i++)
                    {
                        listeners[i].OnEventReceived(eventData);
                    }
                }

                // 触发委托订阅者 - 无异常处理和日志
                if (_eventSubscribers.TryGetValue(eventType, out List<Action<EventData>> subscribers) &&
                    subscribers.Count > 0)
                {
                    for (int i = 0; i < subscribers.Count; i++)
                    {
                        subscribers[i](eventData);
                    }
                }

                return; // 早期退出，避免后续的计数和日志逻辑
            }

            // 正常模式：包含异常处理和日志
            int listenerCount = 0;
            int subscriberCount = 0;

            // 触发监听器对象
            if (_eventListeners.TryGetValue(eventType, out List<IEventListener<EventData>> normalListeners))
            {
                listenerCount = normalListeners.Count;
                if (listenerCount > 0)
                {
                    for (int i = 0; i < normalListeners.Count; i++)
                    {
                        try
                        {
                            normalListeners[i].OnEventReceived(eventData);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"EventManager: Error in event listener: {ex.Message}");
                        }
                    }
                }
            }

            // 触发委托订阅者
            if (_eventSubscribers.TryGetValue(eventType, out List<Action<EventData>> normalSubscribers))
            {
                subscriberCount = normalSubscribers.Count;
                if (subscriberCount > 0)
                {
                    for (int i = 0; i < normalSubscribers.Count; i++)
                    {
                        try
                        {
                            normalSubscribers[i](eventData);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"EventManager: Error in event subscriber: {ex.Message}");
                        }
                    }
                }
            }

            // 只在启用详细日志时输出
            if (_enableVerboseLogging && (listenerCount > 0 || subscriberCount > 0))
            {
                Debug.Log(
                    $"EventManager: Published {eventType.Name} to {listenerCount} listeners and {subscriberCount} subscribers.");
            }
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="listener">事件监听器</param>
        public void Subscribe<T>(IEventListener<T> listener) where T : EventData
        {
            if (listener == null)
            {
                if (_enableVerboseLogging)
                    Debug.LogWarning("EventManager: Trying to subscribe null listener.");
                return;
            }

            Type eventType = typeof(T);

            // 确保监听器同时实现非泛型接口
            if (!(listener is IEventListener<EventData> nonGenericListener))
            {
                Debug.LogError("EventManager: Listener must implement IEventListener interface.");
                return;
            }

            // 添加到事件类型字典
            if (!_eventListeners.ContainsKey(eventType))
            {
                _eventListeners[eventType] = new List<IEventListener<EventData>>();
            }

            if (!_eventListeners[eventType].Contains(nonGenericListener))
            {
                _eventListeners[eventType].Add(nonGenericListener);
            }

            // 添加到监听器字典
            if (!_listenerEvents.ContainsKey(nonGenericListener))
            {
                _listenerEvents[nonGenericListener] = new List<Type>();
            }

            if (!_listenerEvents[nonGenericListener].Contains(eventType))
            {
                _listenerEvents[nonGenericListener].Add(eventType);
            }

            if (_enableVerboseLogging)
                Debug.Log($"EventManager: Subscribed listener to {eventType.Name}.");
        }

        /// <summary>
        /// 取消订阅事件
        /// </summary>
        /// <param name="listener">事件监听器</param>
        public void Unsubscribe(IEventListener<EventData> listener)
        {
            if (listener == null)
            {
                if (_enableVerboseLogging)
                    Debug.LogWarning("EventManager: Trying to unsubscribe null listener.");
                return;
            }

            if (_listenerEvents.TryGetValue(listener, out List<Type> eventTypes))
            {
                foreach (var eventType in eventTypes)
                {
                    if (_eventListeners.ContainsKey(eventType))
                    {
                        _eventListeners[eventType].Remove(listener);

                        // 如果没有监听器了，移除事件类型
                        if (_eventListeners[eventType].Count == 0)
                        {
                            _eventListeners.Remove(eventType);
                        }
                    }
                }

                _listenerEvents.Remove(listener);

                if (_enableVerboseLogging)
                    Debug.Log($"EventManager: Unsubscribed listener from {eventTypes.Count} event types.");
            }
        }

        /// <summary>
        /// 取消订阅特定类型的事件
        /// </summary>
        /// <param name="listener">事件监听器</param>
        /// <param name="eventType">事件类型</param>
        public void Unsubscribe<T>(IEventListener<T> listener, Type eventType) where T : EventData
        {
            if (listener == null || eventType == null)
            {
                Debug.LogWarning("EventManager: Trying to unsubscribe with null parameters.");
                return;
            }

            var castedListener = listener as IEventListener<EventData>;
            if (_eventListeners.ContainsKey(eventType))
            {
                _eventListeners[eventType].Remove(castedListener);

                if (_eventListeners[eventType].Count == 0)
                {
                    _eventListeners.Remove(eventType);
                }
            }

            if (_listenerEvents.ContainsKey(castedListener))
            {
                _listenerEvents[castedListener].Remove(eventType);

                if (_listenerEvents[castedListener].Count == 0)
                {
                    _listenerEvents.Remove(castedListener);
                }
            }

            Debug.Log($"EventManager: Unsubscribed listener from {eventType.Name}.");
        }

        /// <summary>
        /// 获取指定事件类型的监听器数量（包括监听器对象和委托订阅者）
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <returns>总监听器数量</returns>
        public int GetListenerCount(Type eventType)
        {
            int listenerCount = _eventListeners.ContainsKey(eventType) ? _eventListeners[eventType].Count : 0;
            int subscriberCount = _eventSubscribers.ContainsKey(eventType) ? _eventSubscribers[eventType].Count : 0;
            return listenerCount + subscriberCount;
        }

        /// <summary>
        /// 获取指定事件类型的监听器对象数量
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <returns>监听器对象数量</returns>
        public int GetListenerObjectCount(Type eventType)
        {
            return _eventListeners.ContainsKey(eventType) ? _eventListeners[eventType].Count : 0;
        }

        /// <summary>
        /// 获取指定事件类型的委托订阅者数量
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <returns>委托订阅者数量</returns>
        public int GetSubscriberCount(Type eventType)
        {
            return _eventSubscribers.ContainsKey(eventType) ? _eventSubscribers[eventType].Count : 0;
        }

        #region 委托订阅方法

        /// <summary>
        /// 委托方式订阅事件
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="callback">回调函数</param>
        /// <param name="owner">拥有者（可选，用于批量管理）</param>
        public void Subscribe<T>(Action<T> callback, object owner = null) where T : EventData
        {
            if (callback == null)
            {
                if (_enableVerboseLogging)
                    Debug.LogWarning("EventManager: Trying to subscribe null callback.");
                return;
            }

            Type eventType = typeof(T);

            // 包装回调以匹配基类型
            Action<EventData> wrappedCallback = (eventData) =>
            {
                if (eventData is T specificEventData)
                {
                    callback(specificEventData);
                }
            };

            // 添加到委托订阅字典
            if (!_eventSubscribers.ContainsKey(eventType))
            {
                _eventSubscribers[eventType] = new List<Action<EventData>>();
            }

            _eventSubscribers[eventType].Add(wrappedCallback);

            // 如果有拥有者，添加到拥有者管理字典
            if (owner != null)
            {
                if (!_subscriberOwners.ContainsKey(owner))
                {
                    _subscriberOwners[owner] = new List<(Type eventType, Action<EventData> callback)>();
                }

                _subscriberOwners[owner].Add((eventType, wrappedCallback));
            }

            if (_enableVerboseLogging)
                Debug.Log($"EventManager: Subscribed delegate to {eventType.Name}.");
        }

        /// <summary>
        /// 取消委托订阅（需要提供原始回调引用）
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="callback">原始回调函数</param>
        /// <param name="owner">拥有者（可选）</param>
        public void Unsubscribe<T>(Action<T> callback, object owner = null) where T : EventData
        {
            if (callback == null)
            {
                Debug.LogWarning("EventManager: Trying to unsubscribe null callback.");
                return;
            }

            Type eventType = typeof(T);

            // 如果有拥有者信息，通过拥有者来移除
            if (owner != null && _subscriberOwners.ContainsKey(owner))
            {
                var ownerSubscriptions = _subscriberOwners[owner];
                for (int i = ownerSubscriptions.Count - 1; i >= 0; i--)
                {
                    var subscription = ownerSubscriptions[i];
                    if (subscription.eventType == eventType)
                    {
                        // 从委托订阅字典中移除
                        if (_eventSubscribers.ContainsKey(eventType))
                        {
                            _eventSubscribers[eventType].Remove(subscription.callback);

                            if (_eventSubscribers[eventType].Count == 0)
                            {
                                _eventSubscribers.Remove(eventType);
                            }
                        }

                        // 从拥有者字典中移除
                        ownerSubscriptions.RemoveAt(i);
                        break;
                    }
                }

                if (ownerSubscriptions.Count == 0)
                {
                    _subscriberOwners.Remove(owner);
                }

                Debug.Log(
                    $"EventManager: Unsubscribed delegate from {eventType.Name} for owner {owner.GetType().Name}.");
            }
            else
            {
                Debug.LogWarning(
                    $"EventManager: Cannot unsubscribe delegate for {eventType.Name} without owner reference. Use UnsubscribeAll(owner) instead.");
            }
        }

        /// <summary>
        /// 根据拥有者取消所有委托订阅
        /// </summary>
        /// <param name="owner">拥有者对象</param>
        public void UnsubscribeAll(object owner)
        {
            if (owner == null)
            {
                Debug.LogWarning("EventManager: Trying to unsubscribe all with null owner.");
                return;
            }

            if (_subscriberOwners.TryGetValue(owner, out var subscriptions))
            {
                foreach (var (eventType, callback) in subscriptions)
                {
                    if (_eventSubscribers.ContainsKey(eventType))
                    {
                        _eventSubscribers[eventType].Remove(callback);

                        if (_eventSubscribers[eventType].Count == 0)
                        {
                            _eventSubscribers.Remove(eventType);
                        }
                    }
                }

                _subscriberOwners.Remove(owner);
                Debug.Log(
                    $"EventManager: Unsubscribed all delegates for owner {owner.GetType().Name} ({subscriptions.Count} subscriptions).");
            }
        }

        #endregion

        /// <summary>
        /// 清除所有事件监听器（包括监听器对象和委托订阅者）
        /// </summary>
        public void ClearAllListeners()
        {
            _eventListeners.Clear();
            _listenerEvents.Clear();
            _eventSubscribers.Clear();
            _subscriberOwners.Clear();
            Debug.Log("EventManager: Cleared all event listeners and subscribers.");
        }

        protected void OnDestroy()
        {
            ClearAllListeners();
        }
    }
}