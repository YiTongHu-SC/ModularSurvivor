using UnityEngine;

namespace UI.Framework
{
    /// <summary>
    /// 视图接口 - 定义UI视图的基础规范
    /// </summary>
    /// <typeparam name="T">绑定的数据类型</typeparam>
    public interface IView<T>
    {
        /// <summary>
        /// 绑定的数据模型
        /// </summary>
        IModel<T> Model { get; set; }
        
        /// <summary>
        /// 视图的GameObject
        /// </summary>
        GameObject ViewObject { get; }
        
        /// <summary>
        /// 更新视图显示
        /// </summary>
        /// <param name="data">要显示的数据</param>
        void UpdateView(T data);
        
        /// <summary>
        /// 绑定数据模型
        /// </summary>
        /// <param name="model">要绑定的模型</param>
        void BindModel(IModel<T> model);
        
        /// <summary>
        /// 解绑数据模型
        /// </summary>
        void UnbindModel();
        
        /// <summary>
        /// 显示视图
        /// </summary>
        void Show();
        
        /// <summary>
        /// 隐藏视图
        /// </summary>
        void Hide();
        
        /// <summary>
        /// 销毁视图，清理资源
        /// </summary>
        void Dispose();
    }
}
