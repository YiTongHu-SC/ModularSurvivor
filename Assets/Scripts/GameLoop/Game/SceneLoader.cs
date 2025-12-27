using System;
using System.Collections;
using System.Collections.Generic;
using Core.AssetsTool;
using Core.Events;
using LubanGenerated.TableTool;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLoop.Game
{
    public class SceneLoader : MonoBehaviour
    {
        private string _systemSceneName = "SystemScene";
        private Dictionary<GameTransition, string> _transitions;
        private Scene _currentScene;
        private bool _hasScene;
        private GameLoopEvents.LoadingProgressEvent _loadingProgressEvent = new(0.5f, 0.5f);
        private float _loadingSceneTimer;
        private float _minLoadingSceneTime = 3.0f;
        private string _loadingSceneName = "LoadingScene";

        public void Initialize(string systemSceneName, string loadingSceneName,
            List<LoadSceneMap> sceneMaps,
            float minLoadingSceneTime = 3.0f)
        {
            _systemSceneName = systemSceneName;
            _loadingSceneName = loadingSceneName;
            _minLoadingSceneTime = minLoadingSceneTime;
            _transitions ??= new Dictionary<GameTransition, string>();
            _transitions.Clear();

            foreach (var map in sceneMaps)
            {
                _transitions[map.GameTransition] = map.SceneName;
            }
        }

        public void LoadScene(LoadSceneRequest sceneRequest)
        {
            Debug.Log($"Loading scene with : {sceneRequest.GameTransition}");
            StartCoroutine(LoadSceneCoroutine(sceneRequest));
        }

        IEnumerator LoadSceneCoroutine(LoadSceneRequest sceneRequest)
        {
            // 加载Loading场景
            SceneManager.LoadSceneAsync(_loadingSceneName, LoadSceneMode.Additive);
            yield return null;
            // 卸载当前场景
            if (_hasScene)
            {
                yield return UnloadCurrentLevel();
            }
            // load Tables
            TableTool.Initialize();
            // load assets
            var loadTask = AssetSystem.Instance.LoadManifestAsync(sceneRequest.Manifest,
                sceneRequest.ScopeLabel,
                new Progress<float>(TestProgress));
            yield return new WaitUntil(() => loadTask.IsCompleted);
            if (loadTask.IsFaulted)
            {
                Debug.LogError($"Failed to load assets: {loadTask.Exception}");
            }

            _transitions.TryGetValue(sceneRequest.GameTransition, out var sceneName);
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError($"No scene mapped for transition: {sceneRequest.GameTransition}");
                yield break;
            }

            // 异步加载新场景    
            var loadMode = sceneRequest.IsAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single;
            var operation = SceneManager.LoadSceneAsync(sceneName, loadMode);
            if (operation != null)
            {
                operation.allowSceneActivation = false;
                _loadingSceneTimer = 0f;
                var minTimeReached = false;
                var timerProgress = 0f;
                while (operation.progress < 0.9f || !minTimeReached)
                {
                    if (_loadingSceneTimer >= _minLoadingSceneTime)
                    {
                        minTimeReached = true;
                    }

                    _loadingSceneTimer += Time.deltaTime;
                    timerProgress = Mathf.Clamp01(_loadingSceneTimer / _minLoadingSceneTime);
                    // 保证最小加载时间
                    Debug.Log($"Scene loading progress: {operation.progress * 100}%");
                    _loadingProgressEvent.SceneProgress = 0.5f * (operation.progress + timerProgress);
                    _loadingProgressEvent.Message = "Loading Scene Progress";
                    EventManager.Instance.Publish(_loadingProgressEvent);
                    yield return null;
                }

                // 2) 这里可以：播完淡出、等数据预热、等 Addressables 资源等
                yield return new WaitForSeconds(0.2f);
                operation.allowSceneActivation = true;
                yield return new WaitUntil(() => operation.isDone);
                UnloadLoadingScene();
            }

            yield return null;
            _currentScene = SceneManager.GetSceneByName(sceneName);
            _hasScene = _currentScene.IsValid() && _currentScene.isLoaded;
            // Simulate loading delay
            GameManager.Instance.PerformTransition(sceneRequest.GameTransition);
        }

        private void UnloadLoadingScene()
        {
            var loadingScene = SceneManager.GetSceneByName(_loadingSceneName);
            if (loadingScene.IsValid() && loadingScene.isLoaded)
            {
                SceneManager.UnloadSceneAsync(loadingScene);
            }
        }

        private void TestProgress(float progress)
        {
            Debug.Log($"Loading Assets Progress: {progress * 100}%");
            _loadingProgressEvent.AssetProgress = progress;
            _loadingProgressEvent.Message = "Loading Assets Progress";
            EventManager.Instance.Publish(_loadingProgressEvent);
        }

        private IEnumerator UnloadCurrentLevel()
        {
            if (!_hasScene) yield break;
            if (!_currentScene.IsValid() || !_currentScene.isLoaded) yield break;
            // 清理LeanPool对象池
            CleanupLeanPool();
            // 关键点：卸载前先把 ActiveScene 切走（切回 System 或任意已加载场景）
            Scene systemScene = SceneManager.GetSceneByName(_systemSceneName);
            if (systemScene.IsValid() && systemScene.isLoaded)
                SceneManager.SetActiveScene(systemScene);

            // 异步卸载旧关卡
            var op = SceneManager.UnloadSceneAsync(_currentScene);
            while (op != null && !op.isDone) yield return null;

            _hasScene = false;
            _currentScene = default;
        }

        private void CleanupLeanPool()
        {
            Lean.Pool.LeanPool.DespawnAll();
        }
    }

    [Serializable]
    public struct LoadSceneMap
    {
        public GameTransition GameTransition;
        public string SceneName;
    }

    public struct LoadSceneRequest
    {
        public GameTransition GameTransition;
        public AssetManifest Manifest;
        public AssetsScopeLabel ScopeLabel;
        public bool IsAdditive;

        public LoadSceneRequest(GameTransition gameTransition,
            AssetManifest manifest,
            AssetsScopeLabel scopeLabel = default,
            bool isAdditive = true)
        {
            GameTransition = gameTransition;
            Manifest = manifest;
            ScopeLabel = scopeLabel;
            IsAdditive = isAdditive;
        }
    }
}