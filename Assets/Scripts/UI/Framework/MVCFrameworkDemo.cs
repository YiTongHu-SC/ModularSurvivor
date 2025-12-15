using UI.Framework;
using UI.Game.MVC;
using UnityEngine;

namespace UI.Framework
{
    /// <summary>
    /// MVC框架演示脚本
    /// 展示如何在实际项目中使用MVC框架
    /// </summary>
    public class MVCFrameworkDemo : MonoBehaviour
    {
        [Header("Demo Settings")]
        [SerializeField] private bool _enableDemo = true;
        [SerializeField] private float _healthChangeInterval = 2f;
        [SerializeField] private float _damageAmount = 10f;
        [SerializeField] private bool _enableDebugLogs = true;
        
        [Header("UI References")]
        [SerializeField] private GameObject _healthBarPrefab;
        [SerializeField] private Transform _uiParent;
        
        // MVC组件
        private HeroHealthController _heroHealthController;
        private float _lastHealthChangeTime;
        private bool _isDemoRunning;
        
        // 演示数据
        private float _currentHealth = 100f;
        private float _maxHealth = 100f;
        
        #region Unity生命周期
        
        private void Start()
        {
            if (_enableDemo)
            {
                StartDemo();
            }
        }
        
        private void Update()
        {
            if (_isDemoRunning)
            {
                UpdateDemo();
            }
        }
        
        private void OnDestroy()
        {
            StopDemo();
        }
        
        #endregion
        
        #region 演示控制
        
        /// <summary>
        /// 开始MVC演示
        /// </summary>
        [ContextMenu("Start Demo")]
        public void StartDemo()
        {
            if (_isDemoRunning)
            {
                Debug.LogWarning("MVCFrameworkDemo: Demo is already running!");
                return;
            }
            
            Debug.Log("MVCFrameworkDemo: Starting MVC Framework Demo...");
            
            // 初始化UI
            InitializeUI();
            
            // 初始化MVC
            InitializeMVC();
            
            // 启动演示
            _isDemoRunning = true;
            _lastHealthChangeTime = Time.time;
            
            Debug.Log("MVCFrameworkDemo: Demo started successfully!");
            
            // 显示框架信息
            if (_enableDebugLogs)
            {
                ShowFrameworkInfo();
            }
        }
        
        /// <summary>
        /// 停止MVC演示
        /// </summary>
        [ContextMenu("Stop Demo")]
        public void StopDemo()
        {
            if (!_isDemoRunning) return;
            
            _isDemoRunning = false;
            
            // 清理MVC组件
            if (_heroHealthController != null)
            {
                MVCManager.Instance?.UnregisterController(_heroHealthController);
                _heroHealthController = null;
            }
            
            Debug.Log("MVCFrameworkDemo: Demo stopped.");
        }
        
        #endregion
        
        #region MVC初始化
        
        /// <summary>
        /// 初始化UI
        /// </summary>
        private void InitializeUI()
        {
            // 如果没有指定UI父对象，使用当前对象
            if (_uiParent == null)
            {
                _uiParent = transform;
            }
            
            // 创建血量条UI（如果需要）
            if (_healthBarPrefab != null)
            {
                var healthBarInstance = Instantiate(_healthBarPrefab, _uiParent);
                healthBarInstance.name = "DemoHealthBar";
            }
        }
        
        /// <summary>
        /// 初始化MVC组件
        /// </summary>
        private void InitializeMVC()
        {
            // 查找或创建血量条视图
            var healthBarView = FindFirstObjectByType<HeroHpBarView>();
            if (healthBarView == null)
            {
                // 如果没有找到视图，创建一个简单的GameObject并添加视图组件
                var healthBarObject = new GameObject("DemoHealthBar");
                healthBarObject.transform.SetParent(_uiParent);
                healthBarView = healthBarObject.AddComponent<HeroHpBarView>();
                
                Debug.Log("MVCFrameworkDemo: Created HeroHpBarView component.");
            }
            
            // 创建MVC组合
            _heroHealthController = HeroHealthController.CreateMVC(healthBarView.gameObject);
            _heroHealthController.SetDebugLogging(_enableDebugLogs);
            
            // 注册到MVC管理器
            MVCManager.Instance.RegisterController(_heroHealthController);
            
            // 启动控制器
            _heroHealthController.Start();
            
            // 设置初始血量
            _heroHealthController.SetHealthManually(_currentHealth, _maxHealth);
            
            Debug.Log("MVCFrameworkDemo: MVC components initialized.");
        }
        
        #endregion
        
        #region 演示逻辑
        
        /// <summary>
        /// 更新演示逻辑
        /// </summary>
        private void UpdateDemo()
        {
            // 定期改变血量以演示MVC的工作原理
            if (Time.time - _lastHealthChangeTime >= _healthChangeInterval)
            {
                SimulateHealthChange();
                _lastHealthChangeTime = Time.time;
            }
            
            // 更新血量控制器
            _heroHealthController?.UpdateHealth();
        }
        
        /// <summary>
        /// 模拟血量变化
        /// </summary>
        private void SimulateHealthChange()
        {
            // 随机决定是恢复还是受伤
            bool shouldHeal = _currentHealth < _maxHealth * 0.3f || Random.Range(0f, 1f) > 0.7f;
            
            if (shouldHeal && _currentHealth < _maxHealth)
            {
                // 恢复血量
                _currentHealth = Mathf.Min(_maxHealth, _currentHealth + _damageAmount * 0.5f);
                Debug.Log($"MVCFrameworkDemo: Healed! Health: {_currentHealth:F1}/{_maxHealth:F1}");
            }
            else if (_currentHealth > 0)
            {
                // 受到伤害
                _currentHealth = Mathf.Max(0, _currentHealth - _damageAmount);
                Debug.Log($"MVCFrameworkDemo: Damaged! Health: {_currentHealth:F1}/{_maxHealth:F1}");
            }
            else
            {
                // 复活
                _currentHealth = _maxHealth;
                Debug.Log("MVCFrameworkDemo: Revived!");
            }
            
            // 更新MVC模型
            if (_heroHealthController != null)
            {
                _heroHealthController.SetHealthManually(_currentHealth, _maxHealth);
            }
        }
        
        #endregion
        
        #region 调试和信息
        
        /// <summary>
        /// 显示框架信息
        /// </summary>
        [ContextMenu("Show Framework Info")]
        public void ShowFrameworkInfo()
        {
            Debug.Log("=== MVC Framework Demo Info ===");
            Debug.Log($"Demo Running: {_isDemoRunning}");
            Debug.Log($"Current Health: {_currentHealth:F1}/{_maxHealth:F1}");
            
            if (MVCManager.Instance != null)
            {
                Debug.Log(MVCManager.Instance.GetStatistics());
            }
            
            if (_heroHealthController != null)
            {
                var healthData = _heroHealthController.GetCurrentHealthData();
                Debug.Log($"Controller Health Data: {healthData}");
            }
        }
        
        /// <summary>
        /// 手动造成伤害
        /// </summary>
        [ContextMenu("Deal Damage")]
        public void DealDamage()
        {
            DealDamage(_damageAmount);
        }
        
        /// <summary>
        /// 手动造成伤害
        /// </summary>
        /// <param name="damage">伤害值</param>
        public void DealDamage(float damage)
        {
            if (!_isDemoRunning) return;
            
            _currentHealth = Mathf.Max(0, _currentHealth - damage);
            _heroHealthController?.SetHealthManually(_currentHealth, _maxHealth);
            
            Debug.Log($"MVCFrameworkDemo: Manual damage dealt! Health: {_currentHealth:F1}/{_maxHealth:F1}");
        }
        
        /// <summary>
        /// 手动治疗
        /// </summary>
        [ContextMenu("Heal")]
        public void Heal()
        {
            Heal(_damageAmount * 0.5f);
        }
        
        /// <summary>
        /// 手动治疗
        /// </summary>
        /// <param name="healAmount">治疗量</param>
        public void Heal(float healAmount)
        {
            if (!_isDemoRunning) return;
            
            _currentHealth = Mathf.Min(_maxHealth, _currentHealth + healAmount);
            _heroHealthController?.SetHealthManually(_currentHealth, _maxHealth);
            
            Debug.Log($"MVCFrameworkDemo: Manual heal applied! Health: {_currentHealth:F1}/{_maxHealth:F1}");
        }
        
        /// <summary>
        /// 重置血量
        /// </summary>
        [ContextMenu("Reset Health")]
        public void ResetHealth()
        {
            if (!_isDemoRunning) return;
            
            _currentHealth = _maxHealth;
            _heroHealthController?.SetHealthManually(_currentHealth, _maxHealth);
            
            Debug.Log("MVCFrameworkDemo: Health reset to full!");
        }
        
        #endregion
    }
}
