using Core.Units;
using UI.Framework;
using UnityEngine;

namespace UI.Game.MVC
{
    /// <summary>
    /// 英雄血量控制器 - 连接UnitManager和血量UI的桥梁
    /// </summary>
    public class HeroHealthController : BaseController<HeroHealthModel, HeroHpBarView>
    {
        /// <summary>
        /// 血量检查间隔（秒）
        /// </summary>
        private float _updateInterval = 0.1f;

        /// <summary>
        /// 上次更新时间
        /// </summary>
        private float _lastUpdateTime;

        /// <summary>
        /// 上次记录的血量数据，用于检测变化
        /// </summary>
        private HeroHealthData _lastHealthData;

        #region 生命周期

        protected override void OnInitialize()
        {
            base.OnInitialize();

            // 绑定模型到视图
            View.BindModel(Model);

            // 初始化血量数据
            InitializeHealthData();
        }

        protected override void OnStart()
        {
            base.OnStart();

            // 开始监控血量变化
            _lastUpdateTime = Time.time;
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            // 解绑视图
            View?.UnbindModel();
        }

        #endregion

        #region 血量监控

        /// <summary>
        /// 初始化血量数据
        /// </summary>
        private void InitializeHealthData()
        {
            var heroData = GetHeroUnitData();
            if (heroData != null)
            {
                var healthData = new HeroHealthData(heroData.Health, heroData.MaxHealth);
                Model.SetValue(healthData);
                _lastHealthData = healthData;

                if (EnableDebugLogging)
                {
                    Debug.Log($"HeroHealthController: Initialized with health data: {healthData}");
                }
            }
        }

        /// <summary>
        /// 更新血量监控（需要外部调用，比如在Update中）
        /// </summary>
        public void UpdateHealth()
        {
            if (!IsControllerReady() || !IsRunning)
                return;

            // 检查更新间隔
            if (Time.time - _lastUpdateTime < _updateInterval)
                return;

            _lastUpdateTime = Time.time;

            // 检查血量变化
            CheckHealthChanges();
        }

        /// <summary>
        /// 检查血量是否发生变化
        /// </summary>
        private void CheckHealthChanges()
        {
            var heroData = GetHeroUnitData();
            if (heroData == null)
                return;

            var currentHealthData = new HeroHealthData(heroData.Health, heroData.MaxHealth);

            // 检查是否有变化
            if (!IsHealthDataEqual(_lastHealthData, currentHealthData))
            {
                Model.SetValue(currentHealthData);
                _lastHealthData = currentHealthData;

                if (EnableDebugLogging)
                {
                    Debug.Log($"HeroHealthController: Health updated to {currentHealthData}");
                }
            }
        }

        /// <summary>
        /// 比较两个血量数据是否相等
        /// </summary>
        /// <param name="data1">数据1</param>
        /// <param name="data2">数据2</param>
        /// <returns>是否相等</returns>
        private bool IsHealthDataEqual(HeroHealthData data1, HeroHealthData data2)
        {
            return Mathf.Approximately(data1.CurrentHealth, data2.CurrentHealth) &&
                   Mathf.Approximately(data1.MaxHealth, data2.MaxHealth);
        }

        #endregion

        #region 数据访问

        /// <summary>
        /// 获取英雄单位数据
        /// </summary>
        /// <returns>英雄单位数据，如果不存在返回null</returns>
        private UnitData GetHeroUnitData()
        {
            if (UnitManager.Instance == null)
            {
                if (EnableDebugLogging)
                {
                    Debug.LogWarning("HeroHealthController: UnitManager.Instance is null!");
                }

                return null;
            }

            return UnitManager.Instance.HeroUnitData;
        }

        #endregion

        #region 公共接口

        /// <summary>
        /// 设置更新间隔
        /// </summary>
        /// <param name="interval">更新间隔（秒）</param>
        public void SetUpdateInterval(float interval)
        {
            _updateInterval = Mathf.Max(0.01f, interval);
        }

        /// <summary>
        /// 强制刷新血量
        /// </summary>
        public void ForceRefresh()
        {
            SafeExecute(() => { InitializeHealthData(); }, "ForceRefresh");
        }

        /// <summary>
        /// 手动设置血量（用于测试或特殊情况）
        /// </summary>
        /// <param name="currentHealth">当前血量</param>
        /// <param name="maxHealth">最大血量</param>
        public void SetHealthManually(float currentHealth, float maxHealth)
        {
            SafeExecute(() =>
            {
                var heroData = GetHeroUnitData();
                if (heroData == null)
                    return;
                heroData.SetHealth(currentHealth, maxHealth);
                var healthData = new HeroHealthData(heroData.Health, heroData.MaxHealth);
                Model.SetValue(healthData);
                _lastHealthData = healthData;

                if (EnableDebugLogging)
                {
                    Debug.Log($"HeroHealthController: Health manually set to {healthData}");
                }
            }, "SetHealthManually");
        }

        /// <summary>
        /// 获取当前血量数据
        /// </summary>
        /// <returns>当前血量数据</returns>
        public HeroHealthData GetCurrentHealthData()
        {
            return Model?.GetValue() ?? new HeroHealthData(0, 0);
        }

        #endregion

        #region 静态工厂方法

        /// <summary>
        /// 创建完整的英雄血量MVC组合
        /// </summary>
        /// <param name="viewGameObject">视图GameObject</param>
        /// <returns>创建的控制器实例</returns>
        public static HeroHealthController CreateMVC(GameObject viewGameObject)
        {
            // 获取或添加视图组件
            var view = viewGameObject.GetComponent<HeroHpBarView>();
            if (view == null)
            {
                view = viewGameObject.AddComponent<HeroHpBarView>();
            }

            // 创建模型
            var model = new HeroHealthModel(100f, 100f);

            // 创建控制器
            var controller = new HeroHealthController();
            controller.Initialize(model, view);

            return controller;
        }

        #endregion
    }
}