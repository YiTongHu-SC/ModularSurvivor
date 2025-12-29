using System;
using UnityEngine;

namespace UI.Framework
{
    /// <summary>
    /// 数据模型基类 - 集成事件系统的通用数据容器
    /// 提供数据变更通知和EventManager集成
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public abstract class BaseModel<T> : IModel<T>, IDisposable
    {
        /// <summary>
        /// 模型数据值
        /// </summary>
        public abstract T Value { get; protected set; }
        
        /// <summary>
        /// 数据变更事件
        /// </summary>
        public event Action<T> OnValueChanged;
        
        /// <summary>
        /// 模型是否已被销毁
        /// </summary>
        public bool IsDisposed { get; private set; }
        
        /// <summary>
        /// 是否启用调试日志
        /// </summary>
        protected bool EnableDebugLogging;
        
        /// <summary>
        /// 设置数据值并触发变更通知
        /// </summary>
        /// <param name="newValue">新值</param>
        public virtual void SetValue(T newValue, bool forceUpdate = true)
        {
            if (IsDisposed)
            {
                Debug.LogWarning($"BaseModel<{typeof(T).Name}>: Trying to set value on disposed model.");
                return;
            }
            
            // 检查值是否实际发生变化
            if (Equals(Value, newValue) && !forceUpdate)
            {
                return;
            }
            
            T oldValue = Value;
            Value = newValue;
            
            if (EnableDebugLogging)
            {
                Debug.Log($"BaseModel<{typeof(T).Name}>: Value changed from {oldValue} to {newValue}");
            }
            
            // 触发变更事件
            OnValueChanged?.Invoke(newValue);
            
            // 发送全局事件（可选，用于跨模块通信）
            OnValueChangedInternal(oldValue, newValue);
        }
        
        /// <summary>
        /// 获取当前数据值
        /// </summary>
        /// <returns>当前值</returns>
        public virtual T GetValue()
        {
            return Value;
        }
        
        /// <summary>
        /// 内部值变更处理，子类可重写以发送特定事件
        /// </summary>
        /// <param name="oldValue">旧值</param>
        /// <param name="newValue">新值</param>
        protected virtual void OnValueChangedInternal(T oldValue, T newValue)
        {
            // 子类可以重写此方法来发送特定的EventManager事件
        }
        
        /// <summary>
        /// 设置调试日志开关
        /// </summary>
        /// <param name="enabled">是否启用</param>
        public void SetDebugLogging(bool enabled)
        {
            EnableDebugLogging = enabled;
        }
        
        /// <summary>
        /// 销毁模型，清理资源
        /// </summary>
        public virtual void Dispose()
        {
            if (IsDisposed) return;
            
            IsDisposed = true;
            OnValueChanged = null;
            
            if (EnableDebugLogging)
            {
                Debug.Log($"BaseModel<{typeof(T).Name}>: Model disposed.");
            }
        }
        
        /// <summary>
        /// 析构函数，确保资源被清理
        /// </summary>
        ~BaseModel()
        {
            if (!IsDisposed)
            {
                Debug.LogWarning($"BaseModel<{typeof(T).Name}>: Model was not properly disposed!");
                Dispose();
            }
        }
    }
}
