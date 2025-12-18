using Core.Events;
using UnityEngine;

namespace Core.Input
{
    /// <summary>
    ///     输入相关事件定义
    ///     所有输入事件都应该定义在这里，通过 EventManager 发布和订阅
    /// </summary>
    public static class InputEvents
    {
        public enum DebugContext
        {
            ToggleDebugUI // 切换调试UI显示
        }

        /// <summary>
        ///     输入上下文枚举
        /// </summary>
        public enum InputContext
        {
            Gameplay, // 游戏中
            UI, // UI交互
            Paused, // 暂停
            Disabled, // 禁用所有输入
            Debug // 调试模式
        }

        /// <summary>
        ///     交互阶段枚举
        /// </summary>
        public enum InteractionPhase
        {
            Started, // 开始交互
            Performed, // 完成交互（如长按完成）
            Canceled // 取消交互
        }

        /// <summary>
        ///     玩家移动输入事件
        /// </summary>
        public class PlayerMoveInputEvent : EventData
        {
            public PlayerMoveInputEvent(Vector2 rawInput, Vector2 moveDirection)
            {
                RawInput = rawInput;
                MoveDirection = moveDirection;
            }

            /// <summary>
            ///     移动方向向量（归一化）
            ///     X: 左右 (-1 到 1)，Y: 上下 (-1 到 1)
            /// </summary>
            public Vector2 MoveDirection { get; }

            /// <summary>
            ///     原始输入值（可能未归一化）
            /// </summary>
            public Vector2 RawInput { get; }
        }

        /// <summary>
        ///     玩家视角输入事件（鼠标/手柄右摇杆）
        /// </summary>
        public class PlayerLookInputEvent : EventData
        {
            public PlayerLookInputEvent(Vector2 lookDelta)
            {
                LookDelta = lookDelta;
            }

            /// <summary>
            ///     视角变化量（Delta）
            /// </summary>
            public Vector2 LookDelta { get; }
        }

        /// <summary>
        ///     玩家攻击输入事件
        /// </summary>
        public class PlayerAttackInputEvent : EventData
        {
            public PlayerAttackInputEvent(bool isPressed)
            {
                IsPressed = isPressed;
            }

            /// <summary>
            ///     是否按下（true）或释放（false）
            /// </summary>
            public bool IsPressed { get; }
        }

        /// <summary>
        ///     玩家交互输入事件
        /// </summary>
        public class PlayerInteractInputEvent : EventData
        {
            public PlayerInteractInputEvent(InteractionPhase phase, float duration = 0f)
            {
                Phase = phase;
                Duration = duration;
            }

            /// <summary>
            ///     交互类型：Started, Performed, Canceled
            /// </summary>
            public InteractionPhase Phase { get; }

            /// <summary>
            ///     持续时间（用于长按交互）
            /// </summary>
            public float Duration { get; }
        }

        /// <summary>
        ///     玩家冲刺输入事件
        /// </summary>
        public class PlayerSprintInputEvent : EventData
        {
            public PlayerSprintInputEvent(bool isPressed)
            {
                IsPressed = isPressed;
            }

            public bool IsPressed { get; }
        }

        /// <summary>
        ///     玩家跳跃输入事件
        /// </summary>
        public class PlayerJumpInputEvent : EventData
        {
        }

        /// <summary>
        ///     玩家蹲伏输入事件
        /// </summary>
        public class PlayerCrouchInputEvent : EventData
        {
            public PlayerCrouchInputEvent(bool isPressed)
            {
                IsPressed = isPressed;
            }

            public bool IsPressed { get; }
        }

        /// <summary>
        ///     切换到上一个装备/技能
        /// </summary>
        public class PlayerPreviousInputEvent : EventData
        {
        }

        /// <summary>
        ///     切换到下一个装备/技能
        /// </summary>
        public class PlayerNextInputEvent : EventData
        {
        }

        /// <summary>
        ///     输入上下文切换事件（游戏/UI/菜单等）
        /// </summary>
        public class InputContextChangedEvent : EventData
        {
            public InputContextChangedEvent(InputContext context)
            {
                Context = context;
            }

            public InputContext Context { get; }
        }

        public class InputDebugUIEvent : EventData
        {
            public DebugContext Context { get; }

            public InputDebugUIEvent(DebugContext context)
            {
                Context = context;
            }
        }
    }
}