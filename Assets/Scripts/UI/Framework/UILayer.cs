using System;

namespace UI.Framework
{
    /// <summary>
    /// UI层级枚举 - 定义UI显示层级
    /// </summary>
    public enum UILayer
    {
        /// <summary>
        /// 背景层 - 最底层UI元素
        /// </summary>
        Background = 0,

        /// <summary>
        /// HUD层 - 游戏内HUD元素
        /// </summary>
        HUD = 100,

        /// <summary>
        /// 窗口层 - 普通窗口
        /// </summary>
        Window = 200,

        /// <summary>
        /// 弹窗层 - 弹出窗口
        /// </summary>
        Popup = 300,

        /// <summary>
        /// 加载层 - 加载界面
        /// </summary>
        Loading = 400,

        /// <summary>
        /// 系统层 - 系统提示等最高优先级UI
        /// </summary>
        System = 500
    }

    /// <summary>
    /// UI层级属性 - 用于标记UI的层级信息
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class UILayerAttribute : Attribute
    {
        public UILayer Layer { get; }
        public bool BlockInput { get; }
        public bool AllowStack { get; }
        public string ViewKey { get; }

        /// <summary>
        /// UI Attribute for Open Controller
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="viewKey"></param>
        /// <param name="blockInput"></param>
        /// <param name="allowStack">can stack with other UI</param>
        public UILayerAttribute(UILayer layer, string viewKey, bool blockInput = false, bool allowStack = true)
        {
            Layer = layer;
            ViewKey = viewKey;
            BlockInput = blockInput;
            AllowStack = allowStack;
        }
    }
}