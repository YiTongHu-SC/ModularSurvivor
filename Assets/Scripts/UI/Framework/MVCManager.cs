using System;
using System.Collections.Generic;
using Core.Assets;
using StellarCore.Singleton;
using UI.Config;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Framework
{
    /// <summary>
    /// MVC管理器 - 管理所有MVC控制器的生命周期
    /// </summary>
    public class MVCManager : BaseInstance<MVCManager>
    {
        [Header("Settings")] [SerializeField] private bool _enableDebugLogging = true;

        private UIConfig _uiConfig;

        /// <summary>
        /// 所有注册的控制器
        /// </summary>
        private readonly List<IDisposable> _controllers = new List<IDisposable>();

        /// <summary>
        /// 控制器类型映射（用于查找特定类型的控制器）
        /// </summary>
        private readonly Dictionary<Type, List<IDisposable>> _controllersByType =
            new Dictionary<Type, List<IDisposable>>();

        /// <summary>
        /// UI栈管理器
        /// </summary>
        private readonly UIStack _uiStack = new UIStack();

        /// <summary>
        /// UI控制器实例缓存
        /// </summary>
        private readonly Dictionary<Type, IUIController> _uiControllerInstances = new Dictionary<Type, IUIController>();

        /// <summary>
        /// UI层级根节点映射
        /// </summary>
        private readonly Dictionary<UILayer, Transform> _layerRoots = new Dictionary<UILayer, Transform>();

        #region Unity生命周期

        private void Update()
        {
            UpdateControllers();
        }

        private void OnDestroy()
        {
            DisposeAllControllers();
        }

        #endregion

        #region 初始化和清理

        public void Initialize(UIConfig uiConfig)
        {
            _uiConfig = uiConfig;
            Initialize();
        }

        /// <summary>
        /// 初始化管理器
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            InitializeUILayers();
            if (_enableDebugLogging)
            {
                Debug.Log("MVCManager: Initialized.");
            }
        }

        /// <summary>
        /// 初始化UI层级
        /// </summary>
        private void InitializeUILayers()
        {
            // 创建Canvas作为UI根节点
            var uiRoot = new GameObject("UIRoot");
            uiRoot.transform.SetParent(transform);

            var canvas = uiRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 0;

            var canvasScaler = uiRoot.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = _uiConfig ? _uiConfig.DefaultScreenSize : new Vector2(1920, 1080);

            var graphicRaycaster = uiRoot.AddComponent<GraphicRaycaster>();
            graphicRaycaster.ignoreReversedGraphics = true;
            graphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
            graphicRaycaster.blockingMask = LayerMask.GetMask("UI");

            // 为每个UI层级创建根节点
            foreach (UILayer layer in Enum.GetValues(typeof(UILayer)))
            {
                var layerRoot = new GameObject($"Layer_{layer}");
                layerRoot.transform.SetParent(uiRoot.transform);

                var rectTransform = layerRoot.AddComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.sizeDelta = Vector2.zero;
                rectTransform.anchoredPosition = Vector2.zero;

                canvas.sortingOrder = (int)layer;
                _layerRoots[layer] = layerRoot.transform;

                if (_enableDebugLogging)
                {
                    Debug.Log($"MVCManager: Created UI layer {layer} with sorting order {(int)layer}");
                }
            }
        }

        /// <summary>
        /// 销毁所有控制器
        /// </summary>
        private void DisposeAllControllers()
        {
            // 关闭所有UI
            CloseAllUI();

            foreach (var controller in _controllers)
            {
                try
                {
                    controller?.Dispose();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"MVCManager: Error disposing controller - {ex.Message}");
                }
            }

            _controllers.Clear();
            _controllersByType.Clear();
            _uiControllerInstances.Clear();
            _uiStack.Clear();

            if (_enableDebugLogging)
            {
                Debug.Log("MVCManager: All controllers disposed.");
            }
        }

        #endregion

        #region 控制器管理

        /// <summary>
        /// 注册控制器
        /// </summary>
        /// <typeparam name="T">控制器类型</typeparam>
        /// <param name="controller">控制器实例</param>
        /// <returns>是否注册成功</returns>
        public bool RegisterController<T>(T controller) where T : class, IDisposable
        {
            if (controller == null)
            {
                Debug.LogError("MVCManager: Cannot register null controller!");
                return false;
            }

            var type = typeof(T);

            // 添加到总列表
            _controllers.Add(controller);

            // 添加到类型映射
            if (!_controllersByType.ContainsKey(type))
            {
                _controllersByType[type] = new List<IDisposable>();
            }

            _controllersByType[type].Add(controller);

            if (_enableDebugLogging)
            {
                Debug.Log(
                    $"MVCManager: Registered controller of type {type.Name}. Total controllers: {_controllers.Count}");
            }

            return true;
        }

        /// <summary>
        /// 注销控制器
        /// </summary>
        /// <typeparam name="T">控制器类型</typeparam>
        /// <param name="controller">控制器实例</param>
        /// <returns>是否注销成功</returns>
        public bool UnregisterController<T>(T controller) where T : class, IDisposable
        {
            if (controller == null) return false;

            var type = typeof(T);
            bool removed = false;

            // 从总列表移除
            if (_controllers.Remove(controller))
            {
                removed = true;
            }

            // 从类型映射移除
            if (_controllersByType.ContainsKey(type))
            {
                _controllersByType[type].Remove(controller);
                if (_controllersByType[type].Count == 0)
                {
                    _controllersByType.Remove(type);
                }
            }

            // 销毁控制器
            try
            {
                controller.Dispose();
            }
            catch (Exception ex)
            {
                Debug.LogError($"MVCManager: Error disposing controller during unregister - {ex.Message}");
            }

            if (removed && _enableDebugLogging)
            {
                Debug.Log(
                    $"MVCManager: Unregistered controller of type {type.Name}. Total controllers: {_controllers.Count}");
            }

            return removed;
        }

        /// <summary>
        /// 获取指定类型的控制器
        /// </summary>
        /// <typeparam name="T">控制器类型</typeparam>
        /// <returns>找到的控制器列表</returns>
        public List<T> GetControllers<T>() where T : class, IDisposable
        {
            var result = new List<T>();
            var type = typeof(T);

            if (_controllersByType.ContainsKey(type))
            {
                foreach (var controller in _controllersByType[type])
                {
                    if (controller is T typedController)
                    {
                        result.Add(typedController);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 获取第一个指定类型的控制器
        /// </summary>
        /// <typeparam name="T">控制器类型</typeparam>
        /// <returns>找到的控制器，如果没找到返回null</returns>
        public T GetController<T>() where T : class, IDisposable
        {
            var controllers = GetControllers<T>();
            return controllers.Count > 0 ? controllers[0] : null;
        }

        /// <summary>
        /// 检查是否存在指定类型的控制器
        /// </summary>
        /// <typeparam name="T">控制器类型</typeparam>
        /// <returns>是否存在</returns>
        public bool HasController<T>() where T : class, IDisposable
        {
            return GetController<T>() != null;
        }

        #endregion

        #region 控制器生命周期

        /// <summary>
        /// 更新所有控制器（在Update中调用）
        /// </summary>
        private void UpdateControllers()
        {
            // 这里可以调用需要Update的控制器
            // 当前实现为空，控制器如需Update可以实现相应接口
        }

        /// <summary>
        /// 启动所有控制器
        /// </summary>
        public void StartAllControllers()
        {
            foreach (var controller in _controllers)
            {
                if (controller is IController<object, object> baseController)
                {
                    try
                    {
                        baseController.Start();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"MVCManager: Error starting controller - {ex.Message}");
                    }
                }
            }

            if (_enableDebugLogging)
            {
                Debug.Log("MVCManager: All controllers started.");
            }
        }

        /// <summary>
        /// 停止所有控制器
        /// </summary>
        public void StopAllControllers()
        {
            foreach (var controller in _controllers)
            {
                if (controller is IController<object, object> baseController)
                {
                    try
                    {
                        baseController.Stop();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"MVCManager: Error stopping controller - {ex.Message}");
                    }
                }
            }

            if (_enableDebugLogging)
            {
                Debug.Log("MVCManager: All controllers stopped.");
            }
        }

        #endregion

        #region UI Management MVP API

        /// <summary>
        /// 打开指定类型的UI
        /// </summary>
        /// <typeparam name="T">UI控制器类型</typeparam>
        /// <param name="args">打开参数</param>
        /// <returns>是否成功打开</returns>
        public bool Open<T>(object args = null) where T : class, IUIController, new()
        {
            var uiType = typeof(T);
            // 获取UI层级配置
            var layerAttribute = uiType.GetCustomAttributes(typeof(UILayerAttribute), false);
            UILayer layer = UILayer.Window;
            bool allowStack = true;

            if (layerAttribute.Length > 0)
            {
                var attr = (UILayerAttribute)layerAttribute[0];
                layer = attr.Layer;
                allowStack = attr.AllowStack;
            }

            // 检查是否允许堆叠
            if (!allowStack)
            {
                var existingInLayer = _uiStack.GetElementsInLayer(layer);
                if (existingInLayer.Count > 0)
                {
                    if (_enableDebugLogging)
                    {
                        Debug.LogWarning($"MVCManager: UI {uiType.Name} cannot stack in layer {layer}");
                    }

                    return false;
                }
            }

            try
            {
                // 检查是否已经存在实例
                if (_uiControllerInstances.TryGetValue(uiType, out var existingController))
                {
                    if (existingController.IsOpen)
                    {
                        if (_enableDebugLogging)
                        {
                            Debug.LogWarning($"MVCManager: UI {uiType.Name} is already open");
                        }

                        return false;
                    }

                    // 重新打开现有实例
                    OpenUIController(existingController, args);
                    return true;
                }

                // 创建新实例
                var controller = new T();
                controller.InitLayerAttr();
                // 注册控制器
                RegisterController(controller);
                _uiControllerInstances[uiType] = controller;

                // 打开UI
                OpenUIController(controller, args);

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"MVCManager: Error opening UI {uiType.Name} - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 关闭指定类型的UI
        /// </summary>
        /// <typeparam name="T">UI控制器类型</typeparam>
        /// <returns>是否成功关闭</returns>
        public bool Close<T>() where T : class, IUIController
        {
            var uiType = typeof(T);

            try
            {
                if (!_uiControllerInstances.ContainsKey(uiType))
                {
                    if (_enableDebugLogging)
                    {
                        Debug.LogWarning($"MVCManager: UI {uiType.Name} instance not found");
                    }

                    return false;
                }

                var controller = _uiControllerInstances[uiType];
                CloseUIController(controller);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"MVCManager: Error closing UI {uiType.Name} - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 关闭栈顶UI
        /// </summary>
        /// <returns>是否成功关闭</returns>
        public bool CloseTop()
        {
            try
            {
                var topElement = _uiStack.Top;
                if (topElement == null)
                {
                    if (_enableDebugLogging)
                    {
                        Debug.LogWarning("MVCManager: No UI in stack to close");
                    }

                    return false;
                }

                if (topElement.UIInstance is IUIController controller)
                {
                    CloseUIController(controller);
                    return true;
                }

                if (_enableDebugLogging)
                {
                    Debug.LogError($"MVCManager: Top UI {topElement.UIType.Name} is not a valid UI controller");
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.LogError($"MVCManager: Error closing top UI - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 关闭所有UI
        /// </summary>
        public void CloseAllUI()
        {
            var allElements = _uiStack.GetAllElements();

            foreach (var element in allElements)
            {
                if (element.UIInstance is IUIController controller)
                {
                    try
                    {
                        controller.Close();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"MVCManager: Error closing UI {element.UIType.Name} - {ex.Message}");
                    }
                }
            }

            _uiStack.Clear();

            if (_enableDebugLogging)
            {
                Debug.Log("MVCManager: All UI closed");
            }
        }

        /// <summary>
        /// 检查指定UI是否已打开
        /// </summary>
        /// <typeparam name="T">UI控制器类型</typeparam>
        /// <returns>是否已打开</returns>
        public bool IsUIOpen<T>() where T : class, IUIController
        {
            var uiType = typeof(T);
            return _uiControllerInstances.ContainsKey(uiType) && _uiControllerInstances[uiType].IsOpen;
        }

        /// <summary>
        /// 获取当前是否有阻塞输入的UI
        /// </summary>
        /// <returns>是否有阻塞输入的UI</returns>
        public bool HasInputBlocker()
        {
            return _uiStack.HasInputBlocker;
        }

        /// <summary>
        /// 获取UI栈信息
        /// </summary>
        /// <returns>UI栈信息字符串</returns>
        public string GetUIStackInfo()
        {
            var info = $"UI Stack Info (Count: {_uiStack.Count}):\n";
            var elements = _uiStack.GetAllElements();

            for (int i = 0; i < elements.Count; i++)
            {
                var element = elements[i];
                var prefix = i == 0 ? "[TOP] " : "      ";
                info +=
                    $"{prefix}{element.UIType.Name} - Layer: {element.Layer}, BlockInput: {element.BlockInput}, OpenTime: {element.OpenTime:HH:mm:ss}\n";
            }

            return info;
        }

        #endregion

        #region UI Helper Methods

        /// <summary>
        /// 内部方法：打开UI控制器
        /// </summary>
        private void OpenUIController(IUIController controller, object args)
        {
            // 将UI挂到正确的层级

            AttachToLayer(controller, args, () =>
            {
                // 推入UI栈
                var stackElement = new UIStackElement(
                    controller.GetType(),
                    controller,
                    controller.Layer,
                    controller.BlockInput,
                    args
                );

                _uiStack.Push(stackElement);

                // 打开UI
                controller.Open(args);

                if (_enableDebugLogging)
                {
                    Debug.Log($"MVCManager: Opened UI {controller.GetType().Name} in layer {controller.Layer}");
                }
            });
        }

        /// <summary>
        /// 内部方法：关闭UI控制器
        /// </summary>
        private void CloseUIController(IUIController controller)
        {
            // 从UI栈移除
            _uiStack.Remove(controller.GetType());

            // 关闭UI
            controller.Close();

            if (_enableDebugLogging)
            {
                Debug.Log($"MVCManager: Closed UI {controller.GetType().Name}");
            }
        }

        /// <summary>
        /// 将UI挂到正确的层级
        /// </summary>
        private async void AttachToLayer(IUIController controller,
            object args,
            Action callback = null)
        {
            // 这里可以根据实际需求实现UI prefab的加载和层级挂载
            // 当前为简化实现，假设UI已经存在
            if (controller.IsInitialized)
            {
                if (_enableDebugLogging)
                {
                    Debug.Log($"MVCManager: UI {controller.GetType().Name} is already initialized, skipping attach.");
                }

                callback?.Invoke();
            }
            else
            {
                if (!_layerRoots.TryGetValue(controller.Layer, out var layerRoot))
                {
                    Debug.LogError($"MVCManager: Layer root for {controller.Layer} not found!");
                    callback?.Invoke();
                    return;
                }

                if (_enableDebugLogging)
                {
                    Debug.Log($"MVCManager: UI {controller.GetType().Name} attache to layer {controller.Layer}");
                }

                var targetView = await AssetSystem.Instance.Provider.InstantiateAsync(controller.ViewKey,
                    layerRoot,
                    AssetsScopeLabel.Frontend);
                controller.Initialize(targetView, args);
                callback?.Invoke();
            }
        }

        #endregion

        #region 调试和统计

        /// <summary>
        /// 获取控制器统计信息
        /// </summary>
        /// <returns>统计信息字符串</returns>
        public string GetStatistics()
        {
            var stats = $"MVCManager Statistics:\n";
            stats += $"Total Controllers: {_controllers.Count}\n";
            stats += $"UI Controllers: {_uiControllerInstances.Count}\n";
            stats += $"UI Stack Count: {_uiStack.Count}\n";
            stats += $"Controller Types: {_controllersByType.Count}\n";

            foreach (var kvp in _controllersByType)
            {
                stats += $"  {kvp.Key.Name}: {kvp.Value.Count}\n";
            }

            stats += "\n" + GetUIStackInfo();

            return stats;
        }

        /// <summary>
        /// 设置调试日志开关
        /// </summary>
        /// <param name="enable">是否启用调试日志</param>
        public void SetDebugLogging(bool enable)
        {
            _enableDebugLogging = enable;
        }

        #endregion

        public void CreateUI<T>()
        {
            throw new NotImplementedException();
        }
    }
}