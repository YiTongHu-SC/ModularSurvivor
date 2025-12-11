using Core.Abstructs;
using Core.Events;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Input
{
    /// <summary>
    /// 输入管理器 - 集中管理所有玩家输入
    /// 负责监听输入动作，发布输入事件，并提供当前输入状态查询
    /// 支持键盘、手柄、移动端等多平台输入
    /// </summary>
    public class InputManager : BaseInstance<InputManager>
    {
        private InputSystem_Actions _inputActions;
        
        // 当前输入状态缓存（用于状态查询）
        private Vector2 _currentMoveInput;
        private Vector2 _currentLookInput;
        private bool _isAttackPressed;
        private bool _isSprintPressed;
        private bool _isCrouchPressed;
        
        // 输入上下文状态
        private InputEvents.InputContext _currentContext = InputEvents.InputContext.Gameplay;

        #region Lifecycle


        public override void Initialize()
        {
            base.Initialize();
            
            // 创建输入动作实例
            _inputActions = new InputSystem_Actions();
            
            // 订阅所有输入动作
            RegisterPlayerInputCallbacks();
            RegisterUIInputCallbacks();
            
            // 默认启用游戏输入
            EnableGameplayInput();
        }

        private void OnDestroy()
        {
            // 取消订阅，防止内存泄漏
            if (_inputActions != null)
            {
                UnregisterPlayerInputCallbacks();
                UnregisterUIInputCallbacks();
                _inputActions.Dispose();
            }
        }

        #endregion

        #region Input Registration

        /// <summary>
        /// 注册玩家输入回调
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
        /// 注册UI输入回调
        /// </summary>
        private void RegisterUIInputCallbacks()
        {
            // UI 输入由各自的 UI 模块管理（如 DebugPanel）
            // 这里可以预留全局 UI 输入监听
        }

        /// <summary>
        /// 取消注册玩家输入回调
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
        /// 取消注册UI输入回调
        /// </summary>
        private void UnregisterUIInputCallbacks()
        {
            // 对应 RegisterUIInputCallbacks
        }

        #endregion

        #region Input Callbacks - Move & Look

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            Vector2 rawInput = context.ReadValue<Vector2>();
            _currentMoveInput = rawInput;
            
            // 归一化方向（支持手柄摇杆和键盘输入）
            Vector2 normalizedDirection = rawInput.magnitude > 1f ? rawInput.normalized : rawInput;
            
            // 发布移动输入事件
            EventManager.Instance.PublishEvent(
                new InputEvents.PlayerMoveInputEvent(rawInput, normalizedDirection)
            );
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            _currentMoveInput = Vector2.zero;
            
            // 发布停止移动事件
            EventManager.Instance.PublishEvent(
                new InputEvents.PlayerMoveInputEvent(Vector2.zero, Vector2.zero)
            );
        }

        private void OnLookPerformed(InputAction.CallbackContext context)
        {
            Vector2 lookDelta = context.ReadValue<Vector2>();
            _currentLookInput = lookDelta;
            
            // 发布视角输入事件
            EventManager.Instance.PublishEvent(
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
            EventManager.Instance.PublishEvent(new InputEvents.PlayerAttackInputEvent(true));
        }

        private void OnAttackCanceled(InputAction.CallbackContext context)
        {
            _isAttackPressed = false;
            EventManager.Instance.PublishEvent(new InputEvents.PlayerAttackInputEvent(false));
        }

        private void OnSprintPerformed(InputAction.CallbackContext context)
        {
            _isSprintPressed = true;
            EventManager.Instance.PublishEvent(new InputEvents.PlayerSprintInputEvent(true));
        }

        private void OnSprintCanceled(InputAction.CallbackContext context)
        {
            _isSprintPressed = false;
            EventManager.Instance.PublishEvent(new InputEvents.PlayerSprintInputEvent(false));
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            EventManager.Instance.PublishEvent(new InputEvents.PlayerJumpInputEvent());
        }

        private void OnCrouchPerformed(InputAction.CallbackContext context)
        {
            _isCrouchPressed = true;
            EventManager.Instance.PublishEvent(new InputEvents.PlayerCrouchInputEvent(true));
        }

        private void OnCrouchCanceled(InputAction.CallbackContext context)
        {
            _isCrouchPressed = false;
            EventManager.Instance.PublishEvent(new InputEvents.PlayerCrouchInputEvent(false));
        }

        #endregion

        #region Input Callbacks - Interact & Switch

        private void OnInteractStarted(InputAction.CallbackContext context)
        {
            EventManager.Instance.PublishEvent(
                new InputEvents.PlayerInteractInputEvent(InputEvents.InteractionPhase.Started)
            );
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            float duration = (float)context.duration;
            EventManager.Instance.PublishEvent(
                new InputEvents.PlayerInteractInputEvent(InputEvents.InteractionPhase.Performed, duration)
            );
        }

        private void OnInteractCanceled(InputAction.CallbackContext context)
        {
            EventManager.Instance.PublishEvent(
                new InputEvents.PlayerInteractInputEvent(InputEvents.InteractionPhase.Canceled)
            );
        }

        private void OnPreviousPerformed(InputAction.CallbackContext context)
        {
            EventManager.Instance.PublishEvent(new InputEvents.PlayerPreviousInputEvent());
        }

        private void OnNextPerformed(InputAction.CallbackContext context)
        {
            EventManager.Instance.PublishEvent(new InputEvents.PlayerNextInputEvent());
        }

        #endregion

        #region Public Query API - 状态查询接口

        /// <summary>
        /// 获取当前移动输入方向（归一化）
        /// </summary>
        /// <returns>移动方向向量</returns>
        public Vector2 GetMoveDirection()
        {
            return _currentMoveInput.magnitude > 1f ? _currentMoveInput.normalized : _currentMoveInput;
        }

        /// <summary>
        /// 获取原始移动输入值
        /// </summary>
        public Vector2 GetRawMoveInput()
        {
            return _currentMoveInput;
        }

        /// <summary>
        /// 获取视角输入增量
        /// </summary>
        public Vector2 GetLookDelta()
        {
            return _currentLookInput;
        }

        /// <summary>
        /// 是否正在按下攻击键
        /// </summary>
        public bool IsAttackPressed()
        {
            return _isAttackPressed;
        }

        /// <summary>
        /// 是否正在按下冲刺键
        /// </summary>
        public bool IsSprintPressed()
        {
            return _isSprintPressed;
        }

        /// <summary>
        /// 是否正在按下蹲伏键
        /// </summary>
        public bool IsCrouchPressed()
        {
            return _isCrouchPressed;
        }

        /// <summary>
        /// 获取当前输入上下文
        /// </summary>
        public InputEvents.InputContext GetCurrentContext()
        {
            return _currentContext;
        }

        #endregion

        #region Input Context Control - 输入控制

        /// <summary>
        /// 启用游戏输入
        /// </summary>
        public void EnableGameplayInput()
        {
            _inputActions.Player.Enable();
            _currentContext = InputEvents.InputContext.Gameplay;
            EventManager.Instance.PublishEvent(
                new InputEvents.InputContextChangedEvent(InputEvents.InputContext.Gameplay)
            );
            Debug.Log("[InputManager] Gameplay input enabled.");
        }

        /// <summary>
        /// 禁用游戏输入
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

        /// <summary>
        /// 启用UI输入
        /// </summary>
        public void EnableUIInput()
        {
            _inputActions.UI.Enable();
            _currentContext = InputEvents.InputContext.UI;
            EventManager.Instance.PublishEvent(
                new InputEvents.InputContextChangedEvent(InputEvents.InputContext.UI)
            );
            Debug.Log("[InputManager] UI input enabled.");
        }

        /// <summary>
        /// 禁用UI输入
        /// </summary>
        public void DisableUIInput()
        {
            _inputActions.UI.Disable();
            Debug.Log("[InputManager] UI input disabled.");
        }

        /// <summary>
        /// 切换到暂停状态（禁用游戏输入，保留UI输入）
        /// </summary>
        public void SetPausedContext()
        {
            DisableGameplayInput();
            EnableUIInput();
            _currentContext = InputEvents.InputContext.Paused;
            EventManager.Instance.PublishEvent(
                new InputEvents.InputContextChangedEvent(InputEvents.InputContext.Paused)
            );
        }

        /// <summary>
        /// 禁用所有输入
        /// </summary>
        public void DisableAllInput()
        {
            _inputActions.Disable();
            _currentContext = InputEvents.InputContext.Disabled;
            EventManager.Instance.PublishEvent(
                new InputEvents.InputContextChangedEvent(InputEvents.InputContext.Disabled)
            );
            Debug.Log("[InputManager] All input disabled.");
        }

        /// <summary>
        /// 获取底层 InputSystem_Actions 实例（高级用法）
        /// </summary>
        /// <returns>InputSystem_Actions 实例</returns>
        public InputSystem_Actions GetInputActions()
        {
            return _inputActions;
        }

        #endregion
    }
}

