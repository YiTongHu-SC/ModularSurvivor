using Core.Events;
using Core.Input;
using UnityEngine;

namespace Examples
{
    /// <summary>
    ///     InputManager 使用示例
    ///     展示事件订阅和状态查询两种方式
    /// </summary>
    public class InputManagerExample : MonoBehaviour
    {
        [Header("示例配置")] [SerializeField] private float moveSpeed = 5f;

        [SerializeField] private Transform playerTransform;

        private void Start()
        {
            // === 示例1：订阅攻击输入事件 ===
            EventManager.Instance.Subscribe<InputEvents.PlayerAttackInputEvent>(
                OnAttackInput,
                this
            );

            // === 示例2：订阅交互输入事件 ===
            EventManager.Instance.Subscribe<InputEvents.PlayerInteractInputEvent>(
                OnInteractInput,
                this
            );

            // === 示例3：订阅跳跃输入事件 ===
            EventManager.Instance.Subscribe<InputEvents.PlayerJumpInputEvent>(
                OnJumpInput,
                this
            );

            // === 示例4：监听输入上下文切换 ===
            EventManager.Instance.Subscribe<InputEvents.InputContextChangedEvent>(
                OnInputContextChanged,
                this
            );
        }

        private void Update()
        {
            // === 示例5：每帧查询移动输入（推荐方式）===
            var moveDirection = InputManager.Instance.GetMoveDirection();

            if (moveDirection.magnitude > 0.1f && playerTransform != null)
            {
                // 将 2D 输入转换为 3D 移动（XZ平面）
                var movement = new Vector3(moveDirection.x, 0, moveDirection.y);
                playerTransform.Translate(movement * moveSpeed * Time.deltaTime);

                Debug.Log($"玩家移动方向: {moveDirection}");
            }

            // === 示例6：查询按键状态 ===
            if (InputManager.Instance.IsSprintPressed()) Debug.Log("正在冲刺！");
            // 可以提高移动速度
        }

        private void OnDestroy()
        {
            // 清理：批量取消订阅（通过 owner 参数）
            EventManager.Instance.UnsubscribeAll(this);
        }

        #region 事件回调示例

        private void OnAttackInput(InputEvents.PlayerAttackInputEvent eventData)
        {
            if (eventData.IsPressed)
                Debug.Log("攻击键按下 - 触发攻击！");
            // 执行攻击逻辑
            else
                Debug.Log("攻击键释放");
        }

        private void OnInteractInput(InputEvents.PlayerInteractInputEvent eventData)
        {
            switch (eventData.Phase)
            {
                case InputEvents.InteractionPhase.Started:
                    Debug.Log("开始交互（开始长按）");
                    break;

                case InputEvents.InteractionPhase.Performed:
                    Debug.Log($"完成交互（长按 {eventData.Duration:F2} 秒）");
                    // 执行交互逻辑
                    break;

                case InputEvents.InteractionPhase.Canceled:
                    Debug.Log("取消交互（提前松开）");
                    break;
            }
        }

        private void OnJumpInput(InputEvents.PlayerJumpInputEvent eventData)
        {
            Debug.Log("跳跃！");
            // 执行跳跃逻辑
        }

        private void OnInputContextChanged(InputEvents.InputContextChangedEvent eventData)
        {
            Debug.Log($"输入上下文切换到: {eventData.Context}");

            // 根据上下文调整游戏行为
            switch (eventData.Context)
            {
                case InputEvents.InputContext.Gameplay:
                    Debug.Log("恢复游戏控制");
                    break;

                case InputEvents.InputContext.Paused:
                    Debug.Log("游戏暂停，禁用移动");
                    break;

                case InputEvents.InputContext.UI:
                    Debug.Log("UI模式，禁用游戏输入");
                    break;
            }
        }

        #endregion

        #region 输入控制示例

        /// <summary>
        ///     示例：打开暂停菜单时调用
        /// </summary>
        public void OnOpenPauseMenu()
        {
            InputManager.Instance.SetPausedContext();
            Debug.Log("暂停菜单已打开，游戏输入已禁用");
        }

        /// <summary>
        ///     示例：关闭暂停菜单时调用
        /// </summary>
        public void OnClosePauseMenu()
        {
            InputManager.Instance.EnableGameplayInput();
            Debug.Log("暂停菜单已关闭，游戏输入已恢复");
        }

        /// <summary>
        ///     示例：播放过场动画时禁用所有输入
        /// </summary>
        public void OnStartCutscene()
        {
            InputManager.Instance.DisableAllInput();
            Debug.Log("过场动画开始，所有输入已禁用");
        }

        /// <summary>
        ///     示例：过场动画结束后恢复输入
        /// </summary>
        public void OnEndCutscene()
        {
            InputManager.Instance.EnableGameplayInput();
            Debug.Log("过场动画结束，输入已恢复");
        }

        #endregion
    }
}