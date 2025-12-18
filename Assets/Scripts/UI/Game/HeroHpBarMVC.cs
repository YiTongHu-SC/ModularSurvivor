using UI.Framework;
using UnityEngine;

namespace UI.Game
{
    /// <summary>
    /// 英雄血量条MVC组件 - 演示如何使用MVC框架
    /// 这个组件展示了新MVC模式与传统MonoBehaviour的集成
    /// </summary>
    [System.Serializable]
    public class HeroHpBarMVC : MonoBehaviour
    {
        [Header("MVC Components")]
        [SerializeField] private HeroHpBarView _view;
        
        [Header("Settings")]
        [SerializeField] private bool _autoStart = true;
        [SerializeField] private bool _enableDebugLogging;
        
        /// <summary>
        /// MVC控制器
        /// </summary>
        private HeroHealthController _controller;
        
        /// <summary>
        /// 是否已初始化
        /// </summary>
        public bool IsInitialized { get; private set; }
        
        #region Unity生命周期
        
        private void Awake()
        {
            InitializeMVC();
        }
        
        private void Start()
        {
            if (_autoStart)
            {
                StartMVC();
            }
        }
        
        private void Update()
        {
            // 更新血量监控
            _controller?.UpdateHealth();
        }
        
        private void OnDestroy()
        {
            DisposeMVC();
        }
        
        #endregion
        
        #region MVC生命周期
        
        /// <summary>
        /// 初始化MVC组件
        /// </summary>
        private void InitializeMVC()
        {
            if (IsInitialized) return;
            
            // 获取或创建视图组件
            if (_view == null)
            {
                _view = GetComponent<HeroHpBarView>();
                if (_view == null)
                {
                    _view = gameObject.AddComponent<HeroHpBarView>();
                    
                    if (_enableDebugLogging)
                    {
                        Debug.Log($"HeroHpBarMVC: Created HeroHpBarView component on {gameObject.name}");
                    }
                }
            }
            
            // 创建MVC组合
            _controller = HeroHealthController.CreateMVC(gameObject);
            _controller.SetDebugLogging(_enableDebugLogging);
            
            // 注册到MVC管理器
            if (MvcManager.Instance != null)
            {
                MvcManager.Instance.RegisterController(_controller);
            }
            
            IsInitialized = true;
            
            if (_enableDebugLogging)
            {
                Debug.Log($"HeroHpBarMVC: MVC initialized on {gameObject.name}");
            }
        }
        
        /// <summary>
        /// 启动MVC
        /// </summary>
        public void StartMVC()
        {
            if (!IsInitialized)
            {
                InitializeMVC();
            }
            
            _controller?.Start();
            
            if (_enableDebugLogging)
            {
                Debug.Log($"HeroHpBarMVC: MVC started on {gameObject.name}");
            }
        }
        
        /// <summary>
        /// 停止MVC
        /// </summary>
        public void StopMVC()
        {
            _controller?.Stop();
            
            if (_enableDebugLogging)
            {
                Debug.Log($"HeroHpBarMVC: MVC stopped on {gameObject.name}");
            }
        }
        
        /// <summary>
        /// 销毁MVC组件
        /// </summary>
        private void DisposeMVC()
        {
            if (_controller != null)
            {
                // 从管理器注销
                if (MvcManager.Instance != null)
                {
                    MvcManager.Instance.UnregisterController(_controller);
                }
                
                _controller.Dispose();
                _controller = null;
            }
            
            IsInitialized = false;
            
            if (_enableDebugLogging)
            {
                Debug.Log($"HeroHpBarMVC: MVC disposed on {gameObject.name}");
            }
        }
        
        #endregion
        
        #region 公共接口
        
        /// <summary>
        /// 获取控制器实例
        /// </summary>
        /// <returns>控制器实例</returns>
        public HeroHealthController GetController()
        {
            return _controller;
        }
        
        /// <summary>
        /// 获取视图实例
        /// </summary>
        /// <returns>视图实例</returns>
        public HeroHpBarView GetView()
        {
            return _view;
        }
        
        /// <summary>
        /// 手动刷新血量显示
        /// </summary>
        public void RefreshHealth()
        {
            _controller?.ForceRefresh();
        }
        
        /// <summary>
        /// 设置调试日志开关
        /// </summary>
        /// <param name="enable">是否启用调试日志</param>
        public void SetDebugLogging(bool enable)
        {
            _enableDebugLogging = enable;
            _controller?.SetDebugLogging(enable);
            _view?.SetDebugLogging(enable);
        }
        
        /// <summary>
        /// 测试血量设置（仅用于调试）
        /// </summary>
        /// <param name="currentHealth">当前血量</param>
        /// <param name="maxHealth">最大血量</param>
        public void TestSetHealth(float currentHealth, float maxHealth)
        {
            _controller?.SetHealthManually(currentHealth, maxHealth);
        }
        
        #endregion
        
        #region Editor工具方法
        
#if UNITY_EDITOR
        [Header("Editor Testing")]
        [SerializeField, Range(0f, 100f)] private float _testCurrentHealth = 100f;
        [SerializeField, Range(1f, 100f)] private float _testMaxHealth = 100f;
        
        [ContextMenu("Test Health")]
        private void TestHealth()
        {
            if (_controller != null)
            {
                TestSetHealth(_testCurrentHealth, _testMaxHealth);
            }
            else
            {
                Debug.LogWarning("HeroHpBarMVC: Controller not initialized. Cannot test health.");
            }
        }
        
        [ContextMenu("Show MVC Info")]
        private void ShowMVCInfo()
        {
            Debug.Log($"HeroHpBarMVC Info on {gameObject.name}:");
            Debug.Log($"  Initialized: {IsInitialized}");
            Debug.Log($"  Controller: {(_controller != null ? "Created" : "Null")}");
            Debug.Log($"  View: {(_view != null ? "Found" : "Null")}");
            
            if (_controller != null)
            {
                var healthData = _controller.GetCurrentHealthData();
                Debug.Log($"  Current Health: {healthData}");
            }
        }
#endif
        
        #endregion
    }
}
