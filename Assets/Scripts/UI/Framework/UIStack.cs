using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Framework
{
    /// <summary>
    /// UI栈元素 - 表示栈中的一个UI实例
    /// </summary>
    public class UIStackElement
    {
        public Type UIType { get; set; }
        public object UIInstance { get; set; }
        public UILayer Layer { get; set; }
        public bool BlockInput { get; set; }
        public object OpenArgs { get; set; }
        public DateTime OpenTime { get; set; }

        public UIStackElement(Type uiType, object instance, UILayer layer, bool blockInput = false, object openArgs = null)
        {
            UIType = uiType;
            UIInstance = instance;
            Layer = layer;
            BlockInput = blockInput;
            OpenArgs = openArgs;
            OpenTime = DateTime.Now;
        }
    }

    /// <summary>
    /// UI栈管理器 - 管理UI的堆栈显示
    /// </summary>
    public class UIStack
    {
        private readonly Stack<UIStackElement> _stack = new Stack<UIStackElement>();
        private readonly Dictionary<UILayer, List<UIStackElement>> _layerElements = new Dictionary<UILayer, List<UIStackElement>>();

        /// <summary>
        /// 当前栈顶元素
        /// </summary>
        public UIStackElement Top => _stack.Count > 0 ? _stack.Peek() : null;

        /// <summary>
        /// 栈中元素数量
        /// </summary>
        public int Count => _stack.Count;

        /// <summary>
        /// 是否有阻塞输入的UI
        /// </summary>
        public bool HasInputBlocker
        {
            get
            {
                foreach (var element in _stack)
                {
                    if (element.BlockInput) return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 推入一个UI到栈中
        /// </summary>
        public void Push(UIStackElement element)
        {
            if (element == null) return;

            _stack.Push(element);

            // 按层级分组
            if (!_layerElements.ContainsKey(element.Layer))
            {
                _layerElements[element.Layer] = new List<UIStackElement>();
            }
            _layerElements[element.Layer].Add(element);

            Debug.Log($"UIStack: Pushed {element.UIType.Name} to layer {element.Layer}. Stack count: {Count}");
        }

        /// <summary>
        /// 弹出栈顶UI
        /// </summary>
        public UIStackElement Pop()
        {
            if (_stack.Count == 0) return null;

            var element = _stack.Pop();

            // 从层级分组中移除
            if (_layerElements.ContainsKey(element.Layer))
            {
                _layerElements[element.Layer].Remove(element);
                if (_layerElements[element.Layer].Count == 0)
                {
                    _layerElements.Remove(element.Layer);
                }
            }

            Debug.Log($"UIStack: Popped {element.UIType.Name} from layer {element.Layer}. Stack count: {Count}");
            return element;
        }

        /// <summary>
        /// 根据类型查找UI元素
        /// </summary>
        public UIStackElement Find(Type uiType)
        {
            foreach (var element in _stack)
            {
                if (element.UIType == uiType)
                {
                    return element;
                }
            }
            return null;
        }

        /// <summary>
        /// 根据类型移除UI元素
        /// </summary>
        public bool Remove(Type uiType)
        {
            var tempStack = new Stack<UIStackElement>();
            bool found = false;
            UIStackElement targetElement = null;

            // 临时弹出元素直到找到目标
            while (_stack.Count > 0)
            {
                var element = _stack.Pop();
                if (element.UIType == uiType && !found)
                {
                    found = true;
                    targetElement = element;
                    // 不推回目标元素
                }
                else
                {
                    tempStack.Push(element);
                }
            }

            // 将其他元素推回原栈
            while (tempStack.Count > 0)
            {
                _stack.Push(tempStack.Pop());
            }

            // 从层级分组中移除
            if (found && targetElement != null)
            {
                if (_layerElements.ContainsKey(targetElement.Layer))
                {
                    _layerElements[targetElement.Layer].Remove(targetElement);
                    if (_layerElements[targetElement.Layer].Count == 0)
                    {
                        _layerElements.Remove(targetElement.Layer);
                    }
                }
                Debug.Log($"UIStack: Removed {targetElement.UIType.Name} from layer {targetElement.Layer}. Stack count: {Count}");
            }

            return found;
        }

        /// <summary>
        /// 获取指定层级的所有UI元素
        /// </summary>
        public List<UIStackElement> GetElementsInLayer(UILayer layer)
        {
            return _layerElements.ContainsKey(layer) 
                ? new List<UIStackElement>(_layerElements[layer]) 
                : new List<UIStackElement>();
        }

        /// <summary>
        /// 清空栈
        /// </summary>
        public void Clear()
        {
            _stack.Clear();
            _layerElements.Clear();
            Debug.Log("UIStack: Cleared all elements");
        }

        /// <summary>
        /// 获取栈的所有元素（从栈顶到栈底）
        /// </summary>
        public List<UIStackElement> GetAllElements()
        {
            return new List<UIStackElement>(_stack);
        }
    }
}
