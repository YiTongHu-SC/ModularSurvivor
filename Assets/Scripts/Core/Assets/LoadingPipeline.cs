using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Assets
{
    /// <summary>
    /// 加载管线，编排资源加载流程
    /// </summary>
    public class LoadingPipeline
    {
        /// <summary>
        /// 加载阶段信息
        /// </summary>
        public struct LoadingPhase
        {
            public string Name;
            public float Weight;
            public Func<IProgress<float>, Task> LoadFunc;
            
            public LoadingPhase(string name, float weight, Func<IProgress<float>, Task> loadFunc)
            {
                Name = name;
                Weight = weight;
                LoadFunc = loadFunc;
            }
        }
        
        /// <summary>
        /// 加载状态信息
        /// </summary>
        public struct LoadingStatus
        {
            public string CurrentPhase;
            public float PhaseProgress;
            public float OverallProgress;
            public bool IsCompleted;
            public bool HasErrors;
            public string ErrorMessage;
        }
        
        private readonly LoadingPhase[] _phases;
        private readonly float _totalWeight;
        private LoadingStatus _status;
        
        public LoadingStatus Status => _status;
        
        public LoadingPipeline(params LoadingPhase[] phases)
        {
            _phases = phases ?? throw new ArgumentNullException(nameof(phases));
            
            _totalWeight = 0f;
            foreach (var phase in _phases)
            {
                _totalWeight += phase.Weight;
            }
        }
        
        /// <summary>
        /// 执行加载管线
        /// </summary>
        public async Task<bool> ExecuteAsync(IProgress<LoadingStatus> progress = null)
        {
            _status = new LoadingStatus
            {
                CurrentPhase = "",
                PhaseProgress = 0f,
                OverallProgress = 0f,
                IsCompleted = false,
                HasErrors = false,
                ErrorMessage = null
            };
            
            float completedWeight = 0f;
            
            try
            {
                for (int i = 0; i < _phases.Length; i++)
                {
                    var phase = _phases[i];
                    _status.CurrentPhase = phase.Name;
                    _status.PhaseProgress = 0f;
                    _status.OverallProgress = completedWeight / _totalWeight;
                    
                    progress?.Report(_status);
                    
                    // 创建阶段进度报告器
                    var phaseProgress = new Progress<float>(phaseProgressValue =>
                    {
                        _status.PhaseProgress = phaseProgressValue;
                        _status.OverallProgress = (completedWeight + phase.Weight * phaseProgressValue) / _totalWeight;
                        progress?.Report(_status);
                    });
                    
                    // 执行阶段
                    await phase.LoadFunc(phaseProgress);
                    
                    completedWeight += phase.Weight;
                    _status.PhaseProgress = 1f;
                    _status.OverallProgress = completedWeight / _totalWeight;
                    
                    progress?.Report(_status);
                }
                
                _status.IsCompleted = true;
                _status.OverallProgress = 1f;
                progress?.Report(_status);
                
                return true;
            }
            catch (Exception ex)
            {
                _status.HasErrors = true;
                _status.ErrorMessage = ex.Message;
                progress?.Report(_status);
                
                Debug.LogError($"Loading pipeline failed at phase '{_status.CurrentPhase}': {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// 在 MonoBehaviour 中执行（协程版本）
        /// </summary>
        public IEnumerator ExecuteCoroutine(System.Action<LoadingStatus> progressCallback = null)
        {
            var task = ExecuteAsync(progressCallback != null ? new Progress<LoadingStatus>(progressCallback) : null);
            
            yield return new WaitUntil(() => task.IsCompleted);
            
            if (task.Exception != null)
            {
                Debug.LogError($"Loading pipeline failed: {task.Exception.GetBaseException().Message}");
            }
        }
    }
    
    /// <summary>
    /// 常用加载管线构建器
    /// </summary>
    public static class LoadingPipelineBuilder
    {
        /// <summary>
        /// 创建关卡加载管线
        /// </summary>
        public static LoadingPipeline CreateLevelPipeline(
            string levelId, 
            int runId,
            AssetManifest levelManifest = null,
            AssetManifest enemyManifest = null)
        {
            var phases = new System.Collections.Generic.List<LoadingPipeline.LoadingPhase>();
            
            // 场景加载阶段 (30%)
            phases.Add(new LoadingPipeline.LoadingPhase(
                "Loading Scene",
                30f,
                async (progress) =>
                {
                    var scenePath = GetLevelScenePath(levelId);
                    var operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scenePath, UnityEngine.SceneManagement.LoadSceneMode.Additive);
                    
                    while (!operation.isDone)
                    {
                        progress.Report(operation.progress);
                        await Task.Yield();
                    }
                }
            ));
            
            // 关卡资源加载阶段 (40%)
            if (levelManifest != null)
            {
                phases.Add(new LoadingPipeline.LoadingPhase(
                    "Loading Level Assets",
                    40f,
                    async (progress) =>
                    {
                        var scopeName = $"Level_{levelId}_{runId}";
                        await AssetSystem.Instance.LoadManifestAsync(levelManifest, scopeName, progress);
                    }
                ));
            }
            
            // 敌人资源加载阶段 (20%)
            if (enemyManifest != null)
            {
                phases.Add(new LoadingPipeline.LoadingPhase(
                    "Loading Enemy Assets",
                    20f,
                    async (progress) =>
                    {
                        var scopeName = $"Level_{levelId}_{runId}";
                        await AssetSystem.Instance.LoadManifestAsync(enemyManifest, scopeName, progress);
                    }
                ));
            }
            
            // 收尾阶段 (10%)
            phases.Add(new LoadingPipeline.LoadingPhase(
                "Finalizing",
                10f,
                async (progress) =>
                {
                    // 等待一帧确保所有资源就绪
                    await Task.Yield();
                    progress.Report(0.5f);
                    
                    // 可以在这里做一些初始化工作
                    await Task.Yield();
                    progress.Report(1f);
                }
            ));
            
            return new LoadingPipeline(phases.ToArray());
        }
        
        /// <summary>
        /// 创建全局资源加载管线
        /// </summary>
        public static LoadingPipeline CreateGlobalPipeline(AssetManifest globalManifest)
        {
            return new LoadingPipeline(
                new LoadingPipeline.LoadingPhase(
                    "Loading Global Assets",
                    100f,
                    async (progress) =>
                    {
                        await AssetSystem.Instance.LoadManifestAsync(globalManifest, AssetSystem.GlobalScopeName, progress);
                    }
                )
            );
        }
        
        private static string GetLevelScenePath(string levelId)
        {
            // 这里应该根据你的场景命名规则来构建路径
            return $"Level_{levelId}";
        }
    }
}
