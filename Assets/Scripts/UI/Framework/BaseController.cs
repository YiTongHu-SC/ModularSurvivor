using System;
using UnityEngine;

namespace UI.Framework
{
    /// <summary>
    /// 控制器基类 - 连接Model和View，处理业务逻辑
    /// </summary>
    /// <typeparam name="TModel">模型类型</typeparam>
    /// <typeparam name="TView">视图类型</typeparam>
    public abstract class BaseController<TModel, TView> : IController<TModel, TView>, IDisposable
        where TModel : class
        where TView : class
    {

        public int RuntimeId { get; set; }
        /// <summary>
        /// 关联的模型
        /// </summary>
        public TModel Model { get; private set; }

        /// <summary>
        /// 关联的视图
        /// </summary>
        public TView View { get; private set; }

        /// <summary>
        /// 控制器是否已初始化
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// 控制器是否正在运行
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// 控制器是否已销毁
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// 是否启用调试日志
        /// </summary>
        protected bool EnableDebugLogging = false;

        #region 生命周期管理

        /// <summary>
        /// 初始化控制器
        /// </summary>
        /// <param name="model">要关联的模型</param>
        /// <param name="view">要关联的视图</param>
        public virtual void Initialize(TModel model, TView view)
        {
            if (IsInitialized)
            {
                Debug.LogWarning(
                    $"BaseController<{typeof(TModel).Name}, {typeof(TView).Name}>: Controller already initialized!");
                return;
            }

            if (model == null || view == null)
            {
                Debug.LogError(
                    $"BaseController<{typeof(TModel).Name}, {typeof(TView).Name}>: Model or View cannot be null!");
                return;
            }
            RuntimeId = MvcManager.Instance.UIAllocator.Next();
            Model = model;
            View = view;
            IsInitialized = true;

            // 子类特定的初始化逻辑
            OnInitialize();

            if (EnableDebugLogging)
            {
                Debug.Log($"BaseController<{typeof(TModel).Name}, {typeof(TView).Name}>: Controller initialized.");
            }
        }

        /// <summary>
        /// 启动控制器
        /// </summary>
        public virtual void Start()
        {
            if (!IsInitialized)
            {
                Debug.LogError(
                    $"BaseController<{typeof(TModel).Name}, {typeof(TView).Name}>: Controller not initialized!");
                return;
            }

            if (IsRunning)
            {
                Debug.LogWarning(
                    $"BaseController<{typeof(TModel).Name}, {typeof(TView).Name}>: Controller already running!");
                return;
            }

            IsRunning = true;

            // 子类特定的启动逻辑
            OnStart();

            if (EnableDebugLogging)
            {
                Debug.Log($"BaseController<{typeof(TModel).Name}, {typeof(TView).Name}>: Controller started.");
            }
        }

        /// <summary>
        /// 停止控制器
        /// </summary>
        public virtual void Stop()
        {
            if (!IsRunning)
            {
                return;
            }

            IsRunning = false;

            // 子类特定的停止逻辑
            OnStop();

            if (EnableDebugLogging)
            {
                Debug.Log($"BaseController<{typeof(TModel).Name}, {typeof(TView).Name}>: Controller stopped.");
            }
        }

        /// <summary>
        /// 销毁控制器，清理资源
        /// </summary>
        public virtual void Dispose()
        {
            if (IsDisposed) return;

            // 先停止运行
            if (IsRunning)
            {
                Stop();
            }

            // 清理资源
            OnDispose();

            Model = null;
            View = null;
            IsInitialized = false;
            IsDisposed = true;

            if (EnableDebugLogging)
            {
                Debug.Log($"BaseController<{typeof(TModel).Name}, {typeof(TView).Name}>: Controller disposed.");
            }
        }

        #endregion

        #region 生命周期钩子 - 供子类重写

        /// <summary>
        /// 初始化时调用 - 子类可重写
        /// </summary>
        protected virtual void OnInitialize()
        {
            // 子类可重写此方法进行特定初始化
        }

        /// <summary>
        /// 启动时调用 - 子类可重写
        /// </summary>
        protected virtual void OnStart()
        {
            // 子类可重写此方法进行特定启动逻辑
        }

        /// <summary>
        /// 停止时调用 - 子类可重写
        /// </summary>
        protected virtual void OnStop()
        {
            // 子类可重写此方法进行特定停止逻辑
        }

        /// <summary>
        /// 销毁时调用 - 子类必须重写
        /// </summary>
        protected abstract void OnDispose();

        #endregion

        #region 工具方法

        /// <summary>
        /// 设置调试日志开关
        /// </summary>
        /// <param name="enabled">是否启用调试日志</param>
        public void SetDebugLogging(bool enabled)
        {
            EnableDebugLogging = enabled;
        }

        /// <summary>
        /// 检查控制器状态
        /// </summary>
        /// <returns>如果控制器可用返回true</returns>
        protected bool IsControllerReady()
        {
            return IsInitialized && !IsDisposed && Model != null && View != null;
        }

        /// <summary>
        /// 安全执行操作（检查控制器状态）
        /// </summary>
        /// <param name="action">要执行的操作</param>
        /// <param name="actionName">操作名称（用于日志）</param>
        protected void SafeExecute(Action action, string actionName = "Unknown")
        {
            if (!IsControllerReady())
            {
                Debug.LogWarning(
                    $"BaseController<{typeof(TModel).Name}, {typeof(TView).Name}>: Cannot execute {actionName} - controller not ready!");
                return;
            }

            try
            {
                action?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError(
                    $"BaseController<{typeof(TModel).Name}, {typeof(TView).Name}>: Error executing {actionName} - {ex.Message}");
            }
        }

        #endregion

        /// <summary>
        /// 析构函数，确保资源被清理
        /// </summary>
        ~BaseController()
        {
            if (!IsDisposed)
            {
                Debug.LogWarning(
                    $"BaseController<{typeof(TModel).Name}, {typeof(TView).Name}>: Controller was not properly disposed!");
                Dispose();
            }
        }
    }
}