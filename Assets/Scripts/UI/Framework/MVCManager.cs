using System;
using System.Collections.Generic;
using Core.Abstructs;
using UnityEngine;

namespace UI.Framework
{
    /// <summary>
    /// MVC管理器 - 管理所有MVC控制器的生命周期
    /// </summary>
    public class MVCManager : BaseInstance<MVCManager>
    {
        [Header("Settings")] [SerializeField] private bool _enableDebugLogging = true;

        /// <summary>
        /// 所有注册的控制器
        /// </summary>
        private readonly List<IDisposable> _controllers = new List<IDisposable>();

        /// <summary>
        /// 控制器类型映射（用于查找特定类型的控制器）
        /// </summary>
        private readonly Dictionary<Type, List<IDisposable>> _controllersByType =
            new Dictionary<Type, List<IDisposable>>();

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

        /// <summary>
        /// 初始化管理器
        /// </summary>
        private void Initialize()
        {
            if (_enableDebugLogging)
            {
                Debug.Log("MVCManager: Initialized.");
            }
        }

        /// <summary>
        /// 销毁所有控制器
        /// </summary>
        private void DisposeAllControllers()
        {
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

        #region 调试和统计

        /// <summary>
        /// 获取控制器统计信息
        /// </summary>
        /// <returns>统计信息字符串</returns>
        public string GetStatistics()
        {
            var stats = $"MVCManager Statistics:\n";
            stats += $"Total Controllers: {_controllers.Count}\n";
            stats += $"Controller Types: {_controllersByType.Count}\n";

            foreach (var kvp in _controllersByType)
            {
                stats += $"  {kvp.Key.Name}: {kvp.Value.Count}\n";
            }

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
    }
}