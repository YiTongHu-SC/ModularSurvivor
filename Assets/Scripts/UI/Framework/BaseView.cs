using UnityEngine;

namespace UI.Framework
{
    /// <summary>
    /// UI视图基类 - 提供数据绑定和自动更新功能
    /// 继承MonoBehaviour以集成Unity生命周期
    /// </summary>
    /// <typeparam name="T">绑定的数据类型</typeparam>
    public abstract class BaseView<T> : MonoBehaviour, IView<T>
    {
        [Header("View Settings")] [SerializeField]
        private bool AutoUpdate = true;

        [SerializeField] private bool EnableDebugLogging = false;

        /// <summary>
        /// 绑定的数据模型
        /// </summary>
        public IModel<T> Model { get; set; }

        /// <summary>
        /// 视图的GameObject
        /// </summary>
        public GameObject ViewObject => gameObject;

        /// <summary>
        /// 视图是否已初始化
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// 视图是否可见
        /// </summary>
        public bool IsVisible => ViewObject.activeInHierarchy;

        #region Unity生命周期

        protected virtual void Awake()
        {
            InitializeView();
        }

        protected virtual void OnDestroy()
        {
            Dispose();
        }

        #endregion

        #region 初始化和清理

        /// <summary>
        /// 初始化视图
        /// </summary>
        protected virtual void InitializeView()
        {
            if (IsInitialized) return;

            IsInitialized = true;

            if (EnableDebugLogging)
            {
                Debug.Log($"BaseView<{typeof(T).Name}>: View initialized on {gameObject.name}");
            }
        }

        /// <summary>
        /// 销毁视图，清理资源
        /// </summary>
        public virtual void Dispose()
        {
            UnbindModel();
            IsInitialized = false;

            if (EnableDebugLogging)
            {
                Debug.Log($"BaseView<{typeof(T).Name}>: View disposed on {gameObject.name}");
            }
        }

        #endregion

        #region 数据绑定

        /// <summary>
        /// 绑定数据模型
        /// </summary>
        /// <param name="model">要绑定的模型</param>
        public virtual void BindModel(IModel<T> model)
        {
            if (model == null)
            {
                Debug.LogWarning($"BaseView<{typeof(T).Name}>: Trying to bind null model to {gameObject.name}");
                return;
            }

            // 解绑现有模型
            UnbindModel();

            // 绑定新模型
            Model = model;
            Model.OnValueChanged += OnModelValueChanged;

            // 立即更新视图
            if (AutoUpdate)
            {
                UpdateView(Model.GetValue());
            }

            if (EnableDebugLogging)
            {
                Debug.Log($"BaseView<{typeof(T).Name}>: Model bound to {gameObject.name}");
            }
        }

        /// <summary>
        /// 解绑数据模型
        /// </summary>
        public virtual void UnbindModel()
        {
            if (Model != null)
            {
                Model.OnValueChanged -= OnModelValueChanged;
                Model = null;

                if (EnableDebugLogging)
                {
                    Debug.Log($"BaseView<{typeof(T).Name}>: Model unbound from {gameObject.name}");
                }
            }
        }

        /// <summary>
        /// 模型值变更回调
        /// </summary>
        /// <param name="newValue">新值</param>
        protected virtual void OnModelValueChanged(T newValue)
        {
            if (AutoUpdate)
            {
                UpdateView(newValue);
            }

            if (EnableDebugLogging)
            {
                Debug.Log($"BaseView<{typeof(T).Name}>: Model value changed to {newValue} on {gameObject.name}");
            }
        }

        #endregion

        #region 视图操作

        /// <summary>
        /// 更新视图显示 - 子类必须实现
        /// </summary>
        /// <param name="data">要显示的数据</param>
        public abstract void UpdateView(T data);

        /// <summary>
        /// 显示视图
        /// </summary>
        public virtual void Show()
        {
            if (Model == null)
            {
                Debug.LogWarning(
                    $"BaseView<{typeof(T).Name}>: Attempting to show view on {gameObject.name} without a bound model.");
            }

            ViewObject.SetActive(true);

            if (EnableDebugLogging)
            {
                Debug.Log($"BaseView<{typeof(T).Name}>: View shown on {gameObject.name}");
            }
        }

        /// <summary>
        /// 隐藏视图
        /// </summary>
        public virtual void Hide()
        {
            ViewObject.SetActive(false);

            if (EnableDebugLogging)
            {
                Debug.Log($"BaseView<{typeof(T).Name}>: View hidden on {gameObject.name}");
            }
        }

        /// <summary>
        /// 切换视图可见性
        /// </summary>
        public virtual void ToggleVisibility()
        {
            if (IsVisible)
                Hide();
            else
                Show();
        }

        #endregion

        #region 设置和配置

        /// <summary>
        /// 设置自动更新开关
        /// </summary>
        /// <param name="enabled">是否启用自动更新</param>
        public void SetAutoUpdate(bool enabled)
        {
            AutoUpdate = enabled;
        }

        /// <summary>
        /// 设置调试日志开关
        /// </summary>
        /// <param name="enabled">是否启用调试日志</param>
        public void SetDebugLogging(bool enabled)
        {
            EnableDebugLogging = enabled;
        }

        /// <summary>
        /// 手动刷新视图（当autoUpdate关闭时使用）
        /// </summary>
        public void RefreshView()
        {
            if (Model != null)
            {
                UpdateView(Model.GetValue());
            }
        }

        #endregion
    }
}