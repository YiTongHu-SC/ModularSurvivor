using Core.Events;
using StellarCore.Singleton;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Input
{
    /// <summary>
    ///     输入管理器 - 集中管理所有玩家输入
    ///     负责监听输入动作，发布输入事件，并提供当前输入状态查询
    ///     支持键盘、手柄、移动端等多平台输入
    /// </summary>
    public class InputManager : BaseInstance<InputManager>
    {
        // 输入上下文状态
        private InputEvents.InputContext _currentContext = InputEvents.InputContext.Gameplay;
        private Vector2 _currentLookInput;

        // 当前输入状态缓存（用于状态查询）
        private Vector2 _currentMoveInput;
        private InputSystem_Actions _inputActions;
        private bool _isAttackPressed;
        private bool _isCrouchPressed;
        private bool _isSprintPressed;
        public Camera BaseCamera { get; private set; }
        public Camera UiCamera { get; private set; }
        public Camera BattleCamera { get; private set; }

        public void RegisterUICamera(Camera uiCamera)
        {
            UiCamera = uiCamera;
        }

        public void RegisterBaseCamera(Camera baseCamera)
        {
            BaseCamera = baseCamera;
        }

        public void RegisterBattleCamera(Camera battleCamera)
        {
            BattleCamera = battleCamera;
        }

        #region Camera Relative Input

        /// <summary>
        ///     将输入转换为相机相对方向
        ///     Right 方向对应相机的 X 轴，Forward 方向对应相机的 Z 轴
        /// </summary>
        /// <param name="input">原始 2D 输入 (x: 左右, y: 前后)</param>
        /// <returns>转换为相机相对的 2D 方向</returns>
        private Vector2 GetCameraRelativeInput(Vector2 input)
        {
            if (BattleCamera == null)
            {
                // 如果没有相机，返回原始输入
                Debug.LogWarning("[InputManager] MainCamera is null, using raw input");
                return input;
            }

            // 获取相机的前向和右向（忽略 Y 轴，保持在水平面）
            var cameraForward = BattleCamera.transform.forward;
            var cameraRight = BattleCamera.transform.right;

            // 将相机方向投影到水平面（Y = 0）
            cameraForward.y = 0;
            cameraRight.y = 0;

            // 归一化方向向量
            cameraForward.Normalize();
            cameraRight.Normalize();

            // 计算相机相对移动方向
            var moveDirection = cameraRight * input.x + cameraForward * input.y;

            // 转换为 2D 向量（使用 X 和 Z 分量）
            return new Vector2(moveDirection.x, moveDirection.z);
        }

        #endregion

        public void Tick(float deltaTime)
        {
        }

        #region Lifecycle

        public override void Initialize()
        {
            base.Initialize();
            // 创建输入动作实例
            _inputActions = new InputSystem_Actions();

            // 订阅所有输入动作
            RegisterPlayerInputCallbacks();
            RegisterUIInputCallbacks();
            RegisterDebugInputCallbacks();
            // 默认启用游戏输入
            EnableGameplayInput();
            EnableUIInput();
            EnableDebugInput();
        }

        private void OnDestroy()
        {
            // 取消订阅，防止内存泄漏
            if (_inputActions != null)
            {
                UnregisterPlayerInputCallbacks();
                UnregisterUIInputCallbacks();
                UnregisterDebugInputCallbacks();
                _inputActions.Dispose();
            }
        }

        #endregion

        #region Input Registration

        /// <summary>
        ///     注册玩家输入回调
        /// </summary>
        private void RegisterPlayerInputCallbacks()
        {
            _inputActions.Player.Move.performed += OnMovePerformed;
            _inputActions.Player.Move.canceled += OnMoveCanceled;

            _inputActions.Player.Look.performed += OnLookPerformed;
            _inputActions.Player.Look.canceled += OnLookCanceled;

            _inputActions.Player.Attack.performed += OnAttackPerformed;
            _inputActions.Player.Attack.canceled += OnAttackCanceled;

            _inputActions.Player.Sprint.performed += OnSprintPerformed;
            _inputActions.Player.Sprint.canceled += OnSprintCanceled;

            _inputActions.Player.Jump.performed += OnJumpPerformed;

            _inputActions.Player.Crouch.performed += OnCrouchPerformed;
            _inputActions.Player.Crouch.canceled += OnCrouchCanceled;

            _inputActions.Player.Interact.started += OnInteractStarted;
            _inputActions.Player.Interact.performed += OnInteractPerformed;
            _inputActions.Player.Interact.canceled += OnInteractCanceled;

            _inputActions.Player.Previous.performed += OnPreviousPerformed;
            _inputActions.Player.Next.performed += OnNextPerformed;
        }

        /// <summary>
        ///     取消注册玩家输入回调
        /// </summary>
        private void UnregisterPlayerInputCallbacks()
        {
            _inputActions.Player.Move.performed -= OnMovePerformed;
            _inputActions.Player.Move.canceled -= OnMoveCanceled;

            _inputActions.Player.Look.performed -= OnLookPerformed;
            _inputActions.Player.Look.canceled -= OnLookCanceled;

            _inputActions.Player.Attack.performed -= OnAttackPerformed;
            _inputActions.Player.Attack.canceled -= OnAttackCanceled;

            _inputActions.Player.Sprint.performed -= OnSprintPerformed;
            _inputActions.Player.Sprint.canceled -= OnSprintCanceled;

            _inputActions.Player.Jump.performed -= OnJumpPerformed;

            _inputActions.Player.Crouch.performed -= OnCrouchPerformed;
            _inputActions.Player.Crouch.canceled -= OnCrouchCanceled;

            _inputActions.Player.Interact.started -= OnInteractStarted;
            _inputActions.Player.Interact.performed -= OnInteractPerformed;
            _inputActions.Player.Interact.canceled -= OnInteractCanceled;

            _inputActions.Player.Previous.performed -= OnPreviousPerformed;
            _inputActions.Player.Next.performed -= OnNextPerformed;
        }

        /// <summary>
        /// 注册调试输入回调
        /// </summary>
        private void RegisterDebugInputCallbacks()
        {
            _inputActions.Debug.DebugTool.performed += ToggleDebugUI;
        }

        /// <summary>
        /// 取消注册调试输入回调
        /// </summary>
        private void UnregisterDebugInputCallbacks()
        {
            _inputActions.Debug.DebugTool.performed -= ToggleDebugUI;
        }


        /// <summary>
        ///     注册UI输入回调
        /// </summary>
        private void RegisterUIInputCallbacks()
        {
            _inputActions.UI.PauseGame.performed += OnPauseGamePerformed;
        }

        /// <summary>
        ///     取消注册UI输入回调
        /// </summary>
        private void UnregisterUIInputCallbacks()
        {
            _inputActions.UI.PauseGame.performed -= OnPauseGamePerformed;
        }

        #endregion

        #region UI

        private void ToggleDebugUI(InputAction.CallbackContext obj)
        {
            EventManager.Instance.Publish(new InputEvents.InputDebugUIEvent(InputEvents.DebugContext.ToggleDebugUI));
        }

        private void OnPauseGamePerformed(InputAction.CallbackContext context)
        {
            EventManager.Instance.Publish(new InputEvents.InputContextChangedEvent(InputEvents.InputContext.Paused));
        }

        #endregion

        #region Input Callbacks - Move & Look

        /// <summary>
        ///     监听移动输入
        ///     移动输入始终和相机方向相关
        /// </summary>
        /// <param name="context"></param>
        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            var rawInput = context.ReadValue<Vector2>();

            // 转换为相机相对方向
            _currentMoveInput = GetCameraRelativeInput(rawInput);

            // 归一化方向（支持手柄摇杆和键盘输入）
            var normalizedDirection =
                _currentMoveInput.magnitude > 1f ? _currentMoveInput.normalized : _currentMoveInput;

            // 发布移动输入事件（使用原始输入和相机相对方向）
            EventManager.Instance.Publish(
                new InputEvents.PlayerMoveInputEvent(rawInput, normalizedDirection)
            );
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            _currentMoveInput = Vector2.zero;

            // 发布停止移动事件
            EventManager.Instance.Publish(
                new InputEvents.PlayerMoveInputEvent(Vector2.zero, Vector2.zero)
            );
        }

        private void OnLookPerformed(InputAction.CallbackContext context)
        {
            var lookDelta = context.ReadValue<Vector2>();
            _currentLookInput = lookDelta;

            // 发布视角输入事件
            EventManager.Instance.Publish(
                new InputEvents.PlayerLookInputEvent(lookDelta)
            );
        }

        private void OnLookCanceled(InputAction.CallbackContext context)
        {
            _currentLookInput = Vector2.zero;
        }

        #endregion

        #region Input Callbacks - Actions

        private void OnAttackPerformed(InputAction.CallbackContext context)
        {
            _isAttackPressed = true;
            EventManager.Instance.Publish(new InputEvents.PlayerAttackInputEvent(true));
        }

        private void OnAttackCanceled(InputAction.CallbackContext context)
        {
            _isAttackPressed = false;
            EventManager.Instance.Publish(new InputEvents.PlayerAttackInputEvent(false));
        }

        private void OnSprintPerformed(InputAction.CallbackContext context)
        {
            _isSprintPressed = true;
            EventManager.Instance.Publish(new InputEvents.PlayerSprintInputEvent(true));
        }

        private void OnSprintCanceled(InputAction.CallbackContext context)
        {
            _isSprintPressed = false;
            EventManager.Instance.Publish(new InputEvents.PlayerSprintInputEvent(false));
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            EventManager.Instance.Publish(new InputEvents.PlayerJumpInputEvent());
        }

        private void OnCrouchPerformed(InputAction.CallbackContext context)
        {
            _isCrouchPressed = true;
            EventManager.Instance.Publish(new InputEvents.PlayerCrouchInputEvent(true));
        }

        private void OnCrouchCanceled(InputAction.CallbackContext context)
        {
            _isCrouchPressed = false;
            EventManager.Instance.Publish(new InputEvents.PlayerCrouchInputEvent(false));
        }

        #endregion

        #region Input Callbacks - Interact & Switch

        private void OnInteractStarted(InputAction.CallbackContext context)
        {
            EventManager.Instance.Publish(
                new InputEvents.PlayerInteractInputEvent(InputEvents.InteractionPhase.Started)
            );
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            var duration = (float)context.duration;
            EventManager.Instance.Publish(
                new InputEvents.PlayerInteractInputEvent(InputEvents.InteractionPhase.Performed, duration)
            );
        }

        private void OnInteractCanceled(InputAction.CallbackContext context)
        {
            EventManager.Instance.Publish(
                new InputEvents.PlayerInteractInputEvent(InputEvents.InteractionPhase.Canceled)
            );
        }

        private void OnPreviousPerformed(InputAction.CallbackContext context)
        {
            EventManager.Instance.Publish(new InputEvents.PlayerPreviousInputEvent());
        }

        private void OnNextPerformed(InputAction.CallbackContext context)
        {
            EventManager.Instance.Publish(new InputEvents.PlayerNextInputEvent());
        }

        #endregion

        #region Public Query API - 状态查询接口

        /// <summary>
        ///     获取当前移动输入方向（归一化）
        /// </summary>
        /// <returns>移动方向向量</returns>
        public Vector2 GetMoveDirection()
        {
            return _currentMoveInput.magnitude > 1f ? _currentMoveInput.normalized : _currentMoveInput;
        }

        /// <summary>
        ///     获取原始移动输入值
        /// </summary>
        public Vector2 GetRawMoveInput()
        {
            return _currentMoveInput;
        }

        /// <summary>
        ///     获取视角输入增量
        /// </summary>
        public Vector2 GetLookDelta()
        {
            return _currentLookInput;
        }

        /// <summary>
        ///     是否正在按下攻击键
        /// </summary>
        public bool IsAttackPressed()
        {
            return _isAttackPressed;
        }

        /// <summary>
        ///     是否正在按下冲刺键
        /// </summary>
        public bool IsSprintPressed()
        {
            return _isSprintPressed;
        }

        /// <summary>
        ///     是否正在按下蹲伏键
        /// </summary>
        public bool IsCrouchPressed()
        {
            return _isCrouchPressed;
        }

        /// <summary>
        ///     获取当前输入上下文
        /// </summary>
        public InputEvents.InputContext GetCurrentContext()
        {
            return _currentContext;
        }

        #endregion

        #region Input Context Control - 输入控制

        /// <summary>
        ///     启用游戏输入
        /// </summary>
        public void EnableGameplayInput()
        {
            _inputActions.Player.Enable();
            _currentContext = InputEvents.InputContext.Gameplay;
            EventManager.Instance.Publish(
                new InputEvents.InputContextChangedEvent(InputEvents.InputContext.Gameplay)
            );
            Debug.Log("[InputManager] Gameplay input enabled.");
        }

        /// <summary>
        ///     禁用游戏输入
        /// </summary>
        public void DisableGameplayInput()
        {
            _inputActions.Player.Disable();

            // 清空输入状态
            _currentMoveInput = Vector2.zero;
            _currentLookInput = Vector2.zero;
            _isAttackPressed = false;
            _isSprintPressed = false;
            _isCrouchPressed = false;

            Debug.Log("[InputManager] Gameplay input disabled.");
        }

        public void EnableDebugInput()
        {
            // 这里可以启用一些调试输入，比如切换摄像机视角等
            _inputActions.Debug.Enable();
            _currentContext = InputEvents.InputContext.Debug;
            EventManager.Instance.Publish(
                new InputEvents.InputContextChangedEvent(InputEvents.InputContext.Debug)
            );
            Debug.Log("[InputManager] Debug input enabled.");
        }

        public void DisableDebugInput()
        {
            _inputActions.Debug.Disable();
            Debug.Log("[InputManager] Debug input disabled.");
        }

        /// <summary>
        ///     启用UI输入
        /// </summary>
        public void EnableUIInput()
        {
            _inputActions.UI.Enable();
            _currentContext = InputEvents.InputContext.UI;
            EventManager.Instance.Publish(
                new InputEvents.InputContextChangedEvent(InputEvents.InputContext.UI)
            );
            Debug.Log("[InputManager] UI input enabled.");
        }

        /// <summary>
        ///     禁用UI输入
        /// </summary>
        public void DisableUIInput()
        {
            _inputActions.UI.Disable();
            Debug.Log("[InputManager] UI input disabled.");
        }

        /// <summary>
        ///     切换到暂停状态（禁用游戏输入，保留UI输入）
        /// </summary>
        public void SetPausedContext()
        {
            DisableGameplayInput();
            EnableUIInput();
            _currentContext = InputEvents.InputContext.Paused;
            EventManager.Instance.Publish(
                new InputEvents.InputContextChangedEvent(InputEvents.InputContext.Paused)
            );
        }

        /// <summary>
        ///     禁用所有输入
        /// </summary>
        public void DisableAllInput()
        {
            _inputActions.Disable();
            _currentContext = InputEvents.InputContext.Disabled;
            EventManager.Instance.Publish(
                new InputEvents.InputContextChangedEvent(InputEvents.InputContext.Disabled)
            );
            Debug.Log("[InputManager] All input disabled.");
        }

        /// <summary>
        ///     获取底层 InputSystem_Actions 实例（高级用法）
        /// </summary>
        /// <returns>InputSystem_Actions 实例</returns>
        public InputSystem_Actions GetInputActions()
        {
            return _inputActions;
        }

        #endregion
    }
}