namespace UI.Framework
{
    /// <summary>
    /// 控制器接口 - 定义控制器的基础规范
    /// </summary>
    /// <typeparam name="TModel">模型类型</typeparam>
    /// <typeparam name="TView">视图类型</typeparam>
    public interface IController<TModel, TView>
        where TModel : class
        where TView : class
    {
        /// <summary>
        /// 关联的模型
        /// </summary>
        TModel Model { get; }
        
        /// <summary>
        /// 关联的视图
        /// </summary>
        TView View { get; }
        
        /// <summary>
        /// 初始化控制器
        /// </summary>
        /// <param name="model">要关联的模型</param>
        /// <param name="view">要关联的视图</param>
        void Initialize(TModel model, TView view);
        
        /// <summary>
        /// 启动控制器
        /// </summary>
        void Start();
        
        /// <summary>
        /// 停止控制器
        /// </summary>
        void Stop();
        
        /// <summary>
        /// 销毁控制器，清理资源
        /// </summary>
        void Dispose();
    }
}

