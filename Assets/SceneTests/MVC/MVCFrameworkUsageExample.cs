using Core.Input;
using UnityEngine;
using UI.Game;
using UnityEngine.InputSystem;

namespace UI.Framework
{
    /// <summary>
    /// MVC框架使用示例 - 演示如何使用UI管理系统
    /// </summary>
    public class MVCFrameworkUsageExample : MonoBehaviour
    {
        [Header("Demo Settings")] public bool autoStartDemo = true;
        private InputSystem_Actions _inputActions;

        private void Awake()
        {
            _inputActions ??= new InputSystem_Actions();
            _inputActions.UI.Enable();
        }

        private void Start()
        {
            // _inputActions.UI.
            if (autoStartDemo)
            {
                StartDemo();
            }

            _inputActions.UI.OpenMan.performed += OpenMainMenu;
            _inputActions.UI.OpenSettings.performed += OpenSettings;
            _inputActions.UI.CloseTop.performed += ctx => CloseTopUI();
            _inputActions.UI.ShowStackInfo.performed += ctx => ShowStackInfo();
        }

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            // 这里的按键绑定在InputSystem_Actions中定义
        }

        public void StartDemo()
        {
            Debug.Log("=== MVC Framework Demo Started ===");
            Debug.Log("Controls:");
            // 自动打开主菜单作为演示
            OpenMainMenu();
        }

        public void OpenMainMenu(InputAction.CallbackContext callbackContext = default)
        {
            var args = new MainMenuOpenArgs
            {
                UserLevel = 15,
                FromScene = "DemoScene"
            };

            bool success = MVCManager.Instance.Open<MainMenuUIController>(args);
            Debug.Log($"Open MainMenu: {(success ? "Success" : "Failed")}");
        }

        public void OpenSettings(InputAction.CallbackContext callbackContext = default)
        {
            bool success = MVCManager.Instance.Open<SettingsUIController>();
            Debug.Log($"Open Settings: {(success ? "Success" : "Failed")}");
        }

        public void CloseTopUI()
        {
            bool success = MVCManager.Instance.CloseTop();
            Debug.Log($"Close Top UI: {(success ? "Success" : "No UI to close")}");
        }

        public void ShowStackInfo()
        {
            string stackInfo = MVCManager.Instance.GetUIStackInfo();
            Debug.Log("=== UI Stack Info ===\n" + stackInfo);

            Debug.Log($"Has Input Blocker: {MVCManager.Instance.HasInputBlocker()}");

            string stats = MVCManager.Instance.GetStatistics();
            Debug.Log("=== MVC Statistics ===\n" + stats);
        }

        public void CloseAllUI()
        {
            MVCManager.Instance.CloseAllUI();
            Debug.Log("All UI closed");
        }

        public void TestInputBlocking()
        {
            // 演示输入阻塞功能
            Debug.Log("=== Testing Input Blocking ===");

            // 先打开一个非阻塞的UI
            MVCManager.Instance.Open<MainMenuUIController>();
            Debug.Log($"After MainMenu - Has Input Blocker: {MVCManager.Instance.HasInputBlocker()}");

            // 再打开一个阻塞输入的UI
            MVCManager.Instance.Open<SettingsUIController>();
            Debug.Log($"After Settings - Has Input Blocker: {MVCManager.Instance.HasInputBlocker()}");

            // 关闭阻塞UI
            MVCManager.Instance.Close<SettingsUIController>();
            Debug.Log($"After Closing Settings - Has Input Blocker: {MVCManager.Instance.HasInputBlocker()}");
        }

        public void TestStackBehavior()
        {
            // 演示栈行为
            Debug.Log("=== Testing Stack Behavior ===");

            // 清空现有UI
            MVCManager.Instance.CloseAllUI();

            // 打开多个UI测试堆叠
            MVCManager.Instance.Open<MainMenuUIController>();
            MVCManager.Instance.Open<SettingsUIController>();

            ShowStackInfo();

            // 测试CloseTop
            MVCManager.Instance.CloseTop();
            ShowStackInfo();
        }
    }
}