using TMPro;
using UI.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game
{
    /// <summary>
    /// 英雄血量条视图 - MVC模式的血量显示组件
    /// </summary>
    public class HeroHpBarView : BaseView<HeroHealthData>
    {
        [Header("UI References")] [SerializeField]
        private Image _barImage;

        [SerializeField] private TextMeshProUGUI _healthText;
        [SerializeField] private Gradient _healthColorGradient;

        [Header("Animation Settings")] [SerializeField]
        private bool _enableAnimation = true;

        [SerializeField] private float _animationSpeed = 2f;

        // 动画相关
        private float _targetFillAmount;
        private float _currentFillAmount;
        private bool _isAnimating;

        #region Unity生命周期

        protected override void InitializeView()
        {
            base.InitializeView();

            // 验证必需组件
            if (_barImage == null)
            {
                _barImage = GetComponentInChildren<Image>();
                if (_barImage == null)
                {
                    Debug.LogError($"HeroHpBarView: BarImage not found on {gameObject.name}!");
                }
            }

            // 初始化动画状态
            _targetFillAmount = _barImage != null ? _barImage.fillAmount : 1f;
            _currentFillAmount = _targetFillAmount;
        }

        private void Update()
        {
            // 处理血量条动画
            if (_enableAnimation && _isAnimating && _barImage != null)
            {
                UpdateAnimation();
            }
        }

        #endregion

        #region 视图更新

        /// <summary>
        /// 更新血量条显示
        /// </summary>
        /// <param name="data">血量数据</param>
        public override void UpdateView(HeroHealthData data)
        {
            if (_barImage == null) return;

            // 更新血量条填充
            _targetFillAmount = Mathf.Clamp01(data.HealthPercentage);

            if (_enableAnimation)
            {
                StartAnimation();
            }
            else
            {
                // 更新血量条填充量
                _barImage.fillAmount = _targetFillAmount;
                _currentFillAmount = _targetFillAmount;
            }

            // 更新血量条颜色
            UpdateHealthColor(data.HealthPercentage);
            // 更新文本显示
            UpdateHealthText(data);
        }

        /// <summary>
        /// 更新血量条颜色
        /// </summary>
        /// <param name="healthPercentage">血量百分比</param>
        private void UpdateHealthColor(float healthPercentage)
        {
            if (_barImage != null && _healthColorGradient != null)
            {
                _barImage.color = _healthColorGradient.Evaluate(healthPercentage);
            }
        }

        /// <summary>
        /// 更新血量文本
        /// </summary>
        /// <param name="data">血量数据</param>
        private void UpdateHealthText(HeroHealthData data)
        {
            if (_healthText != null)
            {
                _healthText.text = $"{data.CurrentHealth:F0}/{data.MaxHealth:F0}";
            }
        }

        #endregion

        #region 动画处理

        /// <summary>
        /// 开始动画
        /// </summary>
        private void StartAnimation()
        {
            _isAnimating = Mathf.Abs(_currentFillAmount - _targetFillAmount) > 0.001f;
        }

        /// <summary>
        /// 更新动画
        /// </summary>
        private void UpdateAnimation()
        {
            // 计算动画进度
            float animationStep = _animationSpeed * Time.deltaTime;

            // 使用动画曲线进行插值
            _currentFillAmount = Mathf.MoveTowards(_currentFillAmount, _targetFillAmount, animationStep);

            // 应用到UI
            _barImage.fillAmount = _currentFillAmount;

            // 检查动画是否完成
            if (Mathf.Abs(_currentFillAmount - _targetFillAmount) < 0.001f)
            {
                _currentFillAmount = _targetFillAmount;
                _barImage.fillAmount = _targetFillAmount;
                _isAnimating = false;
            }
        }

        #endregion

        #region 配置和设置

        /// <summary>
        /// 设置动画开关
        /// </summary>
        /// <param name="enable">是否启用动画</param>
        public void SetAnimationEnabled(bool enable)
        {
            _enableAnimation = enable;

            // 如果禁用动画，立即设置到目标值
            if (!enable && _barImage != null)
            {
                _barImage.fillAmount = _targetFillAmount;
                _currentFillAmount = _targetFillAmount;
                _isAnimating = false;
            }
        }

        /// <summary>
        /// 设置动画速度
        /// </summary>
        /// <param name="speed">动画速度</param>
        public void SetAnimationSpeed(float speed)
        {
            _animationSpeed = Mathf.Max(0f, speed);
        }

        /// <summary>
        /// 立即设置血量条值（跳过动画）
        /// </summary>
        /// <param name="healthPercentage">血量百分比</param>
        public void SetImmediateValue(float healthPercentage)
        {
            _targetFillAmount = Mathf.Clamp01(healthPercentage);
            _currentFillAmount = _targetFillAmount;

            if (_barImage != null)
            {
                _barImage.fillAmount = _targetFillAmount;
            }

            _isAnimating = false;
            UpdateHealthColor(healthPercentage);
        }

        #endregion

        #region Inspector工具（仅在编辑器中）

#if UNITY_EDITOR
        [Header("Editor Tools")] [SerializeField, Range(0f, 1f)]
        private float _testHealthPercentage = 1f;

        [ContextMenu("Test Health Display")]
        private void TestHealthDisplay()
        {
            var testData = new HeroHealthData(_testHealthPercentage * 100f, 100f);
            UpdateView(testData);
        }
#endif

        #endregion
    }
}