using Core.Units;
using UI.Framework;
using UnityEngine;

namespace UI.Game
{
    /// <summary>
    /// 英雄血量控制器 - 连接UnitManager和血量UI的桥梁
    /// </summary>
    [UILayer(UILayer.HUD, "UI:Prefab:HpBarView", true, true)]
    public class HeroHealthController : BaseUIController<HeroHealthModel, HeroHealthView>
    {
        /// <summary>
        /// 上次记录的血量数据，用于检测变化
        /// </summary>
        private HeroHealthData _lastHealthData;

        #region 生命周期

        public override bool Initialize(GameObject targetView, object args = null)
        {
            if (IsInitialized)
            {
                Debug.LogWarning("HeroHealthController: Controller already initialized!");
                return false;
            }

            if (!targetView.TryGetComponent<HeroHealthView>(out var view))
            {
                Debug.LogError("HeroHealthController: Target view does not have HeroHealthView component!");
                return false;
            }

            var model = new HeroHealthModel();
            view.BindModel(model);
            Initialize(model, view);
            InitializeHealthData();
            return true;
        }

        protected override void OnDispose()
        {
            if (EnableDebugLogging)
            {
                Debug.Log("HeroHealthController: Disposing!");
            }

            Model.Dispose();
            Object.Destroy(View.gameObject);
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
        /// 检查血量是否发生变化
        /// </summary>
        public void Update()
        {
            if (!IsInitialized || !IsOpen) return;
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
    }
}