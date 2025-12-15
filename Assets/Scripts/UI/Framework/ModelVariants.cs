using System;
using Core.Events;
using UnityEngine;

namespace UI.Framework
{
    /// <summary>
    /// 简单数据模型 - 适用于基础数据类型的快速实现
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class SimpleModel<T> : BaseModel<T>
    {
        private T _value;
        
        /// <summary>
        /// 模型数据值
        /// </summary>
        public override T Value 
        { 
            get => _value;
            protected set => _value = value;
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="initialValue">初始值</param>
        public SimpleModel(T initialValue = default(T))
        {
            _value = initialValue;
        }
        
        /// <summary>
        /// 静态工厂方法，创建简单模型
        /// </summary>
        /// <param name="initialValue">初始值</param>
        /// <returns>新的简单模型实例</returns>
        public static SimpleModel<T> Create(T initialValue = default(T))
        {
            return new SimpleModel<T>(initialValue);
        }
    }
    
    /// <summary>
    /// 事件驱动模型 - 集成EventManager的数据模型
    /// 数据变更时自动发送全局事件
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <typeparam name="TEvent">事件类型</typeparam>
    public abstract class EventDrivenModel<T, TEvent> : BaseModel<T>
        where TEvent : EventData, new()
    {
        /// <summary>
        /// 事件创建工厂，子类需要实现
        /// </summary>
        /// <param name="oldValue">旧值</param>
        /// <param name="newValue">新值</param>
        /// <returns>要发送的事件实例</returns>
        protected abstract TEvent CreateEvent(T oldValue, T newValue);
        
        /// <summary>
        /// 重写值变更处理，发送全局事件
        /// </summary>
        /// <param name="oldValue">旧值</param>
        /// <param name="newValue">新值</param>
        protected override void OnValueChangedInternal(T oldValue, T newValue)
        {
            base.OnValueChangedInternal(oldValue, newValue);
            
            try
            {
                var eventData = CreateEvent(oldValue, newValue);
                EventManager.Instance.Publish(eventData);
                
                if (EnableDebugLogging)
                {
                    Debug.Log($"EventDrivenModel<{typeof(T).Name}, {typeof(TEvent).Name}>: Published event for value change.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"EventDrivenModel<{typeof(T).Name}, {typeof(TEvent).Name}>: Failed to publish event - {ex.Message}");
            }
        }
    }
}
