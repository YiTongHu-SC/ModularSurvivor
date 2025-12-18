using System;
using UnityEngine;
using Object = System.Object;

namespace UI.Framework
{
    /// <summary>
    /// 基础UI控制器接口 - 扩展原有IController以支持UI功能
    /// </summary>
    public interface IUIController : IDisposable
    {
        public void InitLayerAttr();

        /// <summary>
        /// 用于标识不同UI的唯一键值，加载View资源
        /// </summary>
        public string ViewKey { get; }

        /// <summary>
        /// 是否已初始化
        /// </summary>
        bool IsInitialized { get; }

        bool Initialize(GameObject targetView, object args = null);

        /// <summary>
        /// UI层级
        /// </summary>
        public UILayer Layer { get; }

        /// <summary>
        /// 是否阻塞输入
        /// </summary>
        bool BlockInput { get; }

        /// <summary>
        /// 是否允许堆叠
        /// </summary>
        bool AllowStack { get; }

        /// <summary>
        /// UI是否已打开
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="args">打开参数</param>
        void Open(object args = null);

        /// <summary>
        /// 关闭UI
        /// </summary>
        void Close();

        /// <summary>
        /// 显示UI
        /// </summary>
        void Show();

        /// <summary>
        /// 隐藏UI
        /// </summary>
        void Hide();
    }

    /// <summary>
    /// 基础UI控制器 - 继承原有BaseController并实现UI功能
    /// </summary>
    /// <typeparam name="TModel">模型类型</typeparam>
    /// <typeparam name="TView">视图类型</typeparam>
    public abstract class BaseUIController<TModel, TView> : IController<TModel, TView>, IUIController
        where TModel : class
        where TView : class
    {
        protected bool _enableDebugLogging = true;

        public TModel Model { get; private set; }
        public TView View { get; private set; }
        public bool IsInitialized { get; private set; }
        public abstract bool Initialize(GameObject targetView, object args = null);
        public UILayer Layer { get; protected set; }
        public bool BlockInput { get; private set; }
        public bool AllowStack { get; private set; }
        public bool IsOpen { get; private set; }
        public string ViewKey { get; protected set; }

        public virtual void InitLayerAttr()
        {
            // 通过Attribute获取UI配置
            var layerAttribute = GetType().GetCustomAttributes(typeof(UILayerAttribute), false);
            if (layerAttribute.Length > 0)
            {
                var attr = (UILayerAttribute)layerAttribute[0];
                Layer = attr.Layer;
                BlockInput = attr.BlockInput;
                AllowStack = attr.AllowStack;
                ViewKey = attr.ViewKey;
            }
            else
            {
                // 默认配置
                Layer = UILayer.Window;
                BlockInput = false;
                AllowStack = true;
            }

            if (_enableDebugLogging)
            {
                Debug.Log(
                    $"BaseUIController: {GetType().Name} configured - Layer: {Layer}, BlockInput: {BlockInput}, AllowStack: {AllowStack}");
            }
        }

        public virtual void Initialize(TModel model, TView view)
        {
            Model = model;
            View = view;
            IsInitialized = true;
            if (_enableDebugLogging)
            {
                Debug.Log($"BaseUIController: {GetType().Name} initialized");
            }
        }

        public virtual void Start()
        {
            if (_enableDebugLogging)
            {
                Debug.Log($"BaseUIController: {GetType().Name} started");
            }
        }

        public virtual void Stop()
        {
            if (_enableDebugLogging)
            {
                Debug.Log($"BaseUIController: {GetType().Name} stopped");
            }
        }

        public virtual void Open(object args = null)
        {
            if (IsOpen)
            {
                if (_enableDebugLogging)
                {
                    Debug.LogWarning($"BaseUIController: {GetType().Name} is already open");
                }

                return;
            }

            OnBeforeOpen(args);
            IsOpen = true;
            Show();
            OnAfterOpen(args);

            if (_enableDebugLogging)
            {
                Debug.Log($"BaseUIController: {GetType().Name} opened with args: {args}");
            }
        }

        public virtual void Close()
        {
            if (!IsOpen)
            {
                if (_enableDebugLogging)
                {
                    Debug.LogWarning($"BaseUIController: {GetType().Name} is already closed");
                }

                return;
            }

            OnBeforeClose();
            IsOpen = false;
            Hide();
            OnAfterClose();

            if (_enableDebugLogging)
            {
                Debug.Log($"BaseUIController: {GetType().Name} closed");
            }
        }

        public virtual void Show()
        {
            if (View is IView<object> baseView)
            {
                baseView.Show();
            }
            else if (View is MonoBehaviour viewMono)
            {
                viewMono.gameObject.SetActive(true);
            }
        }

        public virtual void Hide()
        {
            if (View is IView<object> baseView)
            {
                baseView.Hide();
            }
            else if (View is MonoBehaviour viewMono)
            {
                viewMono.gameObject.SetActive(false);
            }
        }

        public virtual void Dispose()
        {
            if (IsOpen)
            {
                Close();
            }

            OnDispose();

            Model = null;
            View = null;
            IsInitialized = false;

            if (_enableDebugLogging)
            {
                Debug.Log($"BaseUIController: {GetType().Name} disposed");
            }
        }

        /// <summary>
        /// 打开前回调
        /// </summary>
        protected virtual void OnBeforeOpen(object args)
        {
        }

        /// <summary>
        /// 打开后回调
        /// </summary>
        protected virtual void OnAfterOpen(object args)
        {
        }

        /// <summary>
        /// 关闭前回调
        /// </summary>
        protected virtual void OnBeforeClose()
        {
        }

        /// <summary>
        /// 关闭后回调
        /// </summary>
        protected virtual void OnAfterClose()
        {
        }

        /// <summary>
        /// 销毁时回调
        /// </summary>
        protected virtual void OnDispose()
        {
        }
    }
}