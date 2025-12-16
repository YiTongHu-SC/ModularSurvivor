using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Profiling;

namespace Core.Assets
{
    /// <summary>
    /// 内存维护服务，管理资源卸载和垃圾回收策略
    /// </summary>
    public class MemoryMaintenanceService : MonoBehaviour
    {
        private static MemoryMaintenanceService _instance;
        public static MemoryMaintenanceService Instance => _instance;
        
        [SerializeField] private MemoryMaintenanceSettings _settings = MemoryMaintenanceSettings.Default;
        
        private int _levelSwitchCount = 0;
        private float _lastMaintenanceTime = 0f;
        private bool _isMaintenanceRunning = false;
        
        public MemoryMaintenanceSettings Settings => _settings;
        public bool IsMaintenanceRunning => _isMaintenanceRunning;
        public int LevelSwitchCount => _levelSwitchCount;
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// 通知关卡切换
        /// </summary>
        public void NotifyLevelSwitch()
        {
            _levelSwitchCount++;
            
            if (ShouldPerformMaintenance())
            {
                StartCoroutine(PerformMaintenanceCoroutine(_settings.DelayAfterLevelSwitch));
            }
        }
        
        /// <summary>
        /// 通知回到主界面
        /// </summary>
        public void NotifyReturnToMainMenu()
        {
            StartCoroutine(PerformMaintenanceCoroutine(_settings.DelayAfterReturnToMenu));
        }
        
        /// <summary>
        /// 强制执行维护
        /// </summary>
        public void ForceMaintenanceAsync()
        {
            if (!_isMaintenanceRunning)
            {
                StartCoroutine(PerformMaintenanceCoroutine(0f));
            }
        }
        
        /// <summary>
        /// 检查是否应该执行维护
        /// </summary>
        private bool ShouldPerformMaintenance()
        {
            // 如果正在执行维护，跳过
            if (_isMaintenanceRunning)
                return false;
            
            // 检查关卡切换次数
            if (_levelSwitchCount >= _settings.LevelSwitchThreshold)
                return true;
            
            // 检查内存阈值
            if (GetCurrentMemoryUsage() >= _settings.MemoryThreshold)
                return true;
            
            // 检查时间间隔
            if (Time.realtimeSinceStartup - _lastMaintenanceTime >= _settings.TimeInterval)
                return true;
            
            return false;
        }
        
        /// <summary>
        /// 获取当前内存使用量（MB）
        /// </summary>
        private float GetCurrentMemoryUsage()
        {
            // 获取Unity分配的内存
            var unityMemory = Profiler.GetTotalAllocatedMemoryLong() / (1024f * 1024f);
            return unityMemory;
        }
        
        /// <summary>
        /// 执行内存维护协程
        /// </summary>
        private IEnumerator PerformMaintenanceCoroutine(float delay)
        {
            _isMaintenanceRunning = true;
            
            var startTime = Time.realtimeSinceStartup;
            var startMemory = GetCurrentMemoryUsage();
            
            Debug.Log($"[MemoryMaintenance] Starting maintenance. Memory: {startMemory:F2}MB, Level switches: {_levelSwitchCount}");
            
            // 延迟执行
            if (delay > 0f)
            {
                yield return new WaitForSeconds(delay);
            }
            
            // 执行UnloadUnusedAssets
            var unloadOperation = Resources.UnloadUnusedAssets();
            yield return unloadOperation;
            
            // 执行垃圾回收
            if (_settings.EnableGarbageCollection)
            {
                GC.Collect();
                yield return null; // 等待一帧
            }
            
            // 重置计数器
            _levelSwitchCount = 0;
            _lastMaintenanceTime = Time.realtimeSinceStartup;
            _isMaintenanceRunning = false;
            
            var endTime = Time.realtimeSinceStartup;
            var endMemory = GetCurrentMemoryUsage();
            var duration = endTime - startTime;
            var memoryReduction = startMemory - endMemory;
            
            Debug.Log($"[MemoryMaintenance] Completed. Duration: {duration:F2}s, Memory: {endMemory:F2}MB (reduced by {memoryReduction:F2}MB)");
        }
        
        /// <summary>
        /// 获取内存统计信息
        /// </summary>
        public MemoryStats GetMemoryStats()
        {
            return new MemoryStats
            {
                TotalAllocatedMemory = Profiler.GetTotalAllocatedMemoryLong() / (1024f * 1024f),
                TotalReservedMemory = Profiler.GetTotalReservedMemoryLong() / (1024f * 1024f),
                TotalUnusedReservedMemory = Profiler.GetTotalUnusedReservedMemoryLong() / (1024f * 1024f),
                MonoHeapSize = Profiler.GetMonoHeapSizeLong() / (1024f * 1024f),
                MonoUsedSize = Profiler.GetMonoUsedSizeLong() / (1024f * 1024f),
                LevelSwitchCount = _levelSwitchCount,
                LastMaintenanceTime = _lastMaintenanceTime,
                IsMaintenanceRunning = _isMaintenanceRunning
            };
        }
    }
    
    /// <summary>
    /// 内存维护设置
    /// </summary>
    [Serializable]
    public class MemoryMaintenanceSettings
    {
        [Header("Trigger Conditions")]
        [Tooltip("关卡切换多少次后触发维护")]
        public int LevelSwitchThreshold = 3;
        
        [Tooltip("内存使用量超过多少MB时触发维护")]
        public float MemoryThreshold = 500f;
        
        [Tooltip("距离上次维护多少秒后可以再次触发")]
        public float TimeInterval = 300f; // 5分钟
        
        [Header("Execution Settings")]
        [Tooltip("关卡切换后延迟多少秒执行维护")]
        public float DelayAfterLevelSwitch = 2f;
        
        [Tooltip("回到主界面后延迟多少秒执行维护")]
        public float DelayAfterReturnToMenu = 5f;
        
        [Tooltip("是否同时执行垃圾回收")]
        public bool EnableGarbageCollection = true;
        
        public static MemoryMaintenanceSettings Default => new MemoryMaintenanceSettings();
    }
    
    /// <summary>
    /// 内存统计信息
    /// </summary>
    public struct MemoryStats
    {
        public float TotalAllocatedMemory;    // Unity分配的总内存(MB)
        public float TotalReservedMemory;     // Unity保留的总内存(MB)
        public float TotalUnusedReservedMemory; // Unity未使用的保留内存(MB)
        public float MonoHeapSize;            // Mono堆大小(MB)
        public float MonoUsedSize;            // Mono已使用大小(MB)
        public int LevelSwitchCount;          // 当前关卡切换计数
        public float LastMaintenanceTime;     // 上次维护时间
        public bool IsMaintenanceRunning;     // 是否正在执行维护
        
        public override string ToString()
        {
            return $"Memory Stats - Allocated: {TotalAllocatedMemory:F2}MB, Reserved: {TotalReservedMemory:F2}MB, " +
                   $"Mono Heap: {MonoHeapSize:F2}MB, Level Switches: {LevelSwitchCount}, " +
                   $"Maintenance Running: {IsMaintenanceRunning}";
        }
    }
}
