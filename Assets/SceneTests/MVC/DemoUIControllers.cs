using UnityEngine;
using UI.Framework;

namespace UI.Game
{
    /// <summary>
    /// 示例主菜单UI控制器 - 演示如何使用MVC框架
    /// </summary>
    [UILayer(UILayer.Window, "ui", blockInput: false, allowStack: false)]
    public class MainMenuUIController : BaseUIController<MainMenuModel, MainMenuView>
    {
        public override bool Initialize(GameObject targetView, object args = null)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnBeforeOpen(object args)
        {
            // 打开前的逻辑，比如初始化数据
            if (args is MainMenuOpenArgs openArgs)
            {
                Debug.Log($"MainMenuUI: Opening with user level {openArgs.UserLevel}");
            }
        }

        protected override void OnAfterOpen(object args)
        {
            // 打开后的逻辑，比如播放动画
            Debug.Log("MainMenuUI: Opened successfully");
        }

        protected override void OnBeforeClose()
        {
            // 关闭前的逻辑，比如保存状态
            Debug.Log("MainMenuUI: Closing...");
        }

        protected override void OnAfterClose()
        {
            // 关闭后的逻辑
            Debug.Log("MainMenuUI: Closed successfully");
        }

        protected override void OnDispose()
        {
            throw new System.NotImplementedException();
        }
    }

    /// <summary>
    /// 主菜单数据模型
    /// </summary>
    public class MainMenuModel
    {
        public string PlayerName { get; set; }
        public int PlayerLevel { get; set; }
        public int Currency { get; set; }
    }

    /// <summary>
    /// 主菜单视图
    /// </summary>
    public class MainMenuView : MonoBehaviour
    {
        [Header("UI Components")] public UnityEngine.UI.Text playerNameText;
        public UnityEngine.UI.Text playerLevelText;
        public UnityEngine.UI.Text currencyText;
        public UnityEngine.UI.Button startGameButton;
        public UnityEngine.UI.Button settingsButton;
        public UnityEngine.UI.Button exitButton;

        private void Start()
        {
            // 绑定按钮事件
            if (startGameButton != null)
                startGameButton.onClick.AddListener(OnStartGameClicked);

            if (settingsButton != null)
                settingsButton.onClick.AddListener(OnSettingsClicked);

            if (exitButton != null)
                exitButton.onClick.AddListener(OnExitClicked);
        }

        public void UpdateView(MainMenuModel model)
        {
            if (playerNameText != null)
                playerNameText.text = model.PlayerName;

            if (playerLevelText != null)
                playerLevelText.text = $"Level: {model.PlayerLevel}";

            if (currencyText != null)
                currencyText.text = $"Coins: {model.Currency}";
        }

        private void OnStartGameClicked()
        {
            Debug.Log("Start Game button clicked");
            // 这里可以通过事件系统通知控制器
        }

        private void OnSettingsClicked()
        {
            Debug.Log("Settings button clicked");
            // 打开设置界面
            MvcManager.Instance.Open<SettingsUIController>();
        }

        private void OnExitClicked()
        {
            Debug.Log("Exit button clicked");
            Application.Quit();
        }
    }

    /// <summary>
    /// 主菜单打开参数
    /// </summary>
    public class MainMenuOpenArgs
    {
        public int UserLevel { get; set; }
        public string FromScene { get; set; }
    }
}

namespace UI.Game
{
    /// <summary>
    /// 示例设置UI控制器 - 演示弹窗层级
    /// </summary>
    [UILayer(UILayer.Popup, "settings", blockInput: true, allowStack: true)]
    public class SettingsUIController : BaseUIController<SettingsModel, SettingsView>
    {
        public override bool Initialize(GameObject targetView, object args = null)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnAfterOpen(object args)
        {
            Debug.Log("SettingsUI: Popup opened, input blocked");
        }

        protected override void OnAfterClose()
        {
            Debug.Log("SettingsUI: Popup closed, input unblocked");
        }

        protected override void OnDispose()
        {
            throw new System.NotImplementedException();
        }
    }

    /// <summary>
    /// 设置数据模型
    /// </summary>
    public class SettingsModel
    {
        public float MasterVolume { get; set; } = 1.0f;
        public float SfxVolume { get; set; } = 1.0f;
        public bool EnableVSync { get; set; } = true;
    }

    /// <summary>
    /// 设置视图
    /// </summary>
    public class SettingsView : MonoBehaviour
    {
        [Header("Settings UI")] public UnityEngine.UI.Slider masterVolumeSlider;
        public UnityEngine.UI.Slider sfxVolumeSlider;
        public UnityEngine.UI.Toggle vsyncToggle;
        public UnityEngine.UI.Button closeButton;

        private void Start()
        {
            if (closeButton != null)
                closeButton.onClick.AddListener(() => { MvcManager.Instance.Close<SettingsUIController>(); });
        }

        public void UpdateView(SettingsModel model)
        {
            if (masterVolumeSlider != null)
                masterVolumeSlider.value = model.MasterVolume;

            if (sfxVolumeSlider != null)
                sfxVolumeSlider.value = model.SfxVolume;

            if (vsyncToggle != null)
                vsyncToggle.isOn = model.EnableVSync;
        }
    }
}