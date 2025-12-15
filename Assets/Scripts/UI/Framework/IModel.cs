using System;

namespace UI.Framework
{
    /// <summary>
    /// 模型接口 - 定义数据模型的基础规范
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public interface IModel<T>
    {
        /// <summary>
        /// 模型数据值
        /// </summary>
        T Value { get; }
        
        /// <summary>
        /// 数据变更事件
        /// </summary>
        event Action<T> OnValueChanged;
        
        /// <summary>
        /// 设置数据值
        /// </summary>
        /// <param name="newValue">新值</param>
        void SetValue(T newValue);
        
        /// <summary>
        /// 获取数据值
        /// </summary>
        /// <returns>当前值</returns>
        T GetValue();
        
        /// <summary>
        /// 销毁模型，清理资源
        /// </summary>
        void Dispose();
    }
}
