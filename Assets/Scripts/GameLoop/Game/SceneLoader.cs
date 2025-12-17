using System;
using System.Collections;
using System.Collections.Generic;
using Core.Assets;
using Core.AssetsTool;
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

        public void Initialize(string systemSceneName, List<LoadSceneMap> sceneMaps)
        {
            _systemSceneName = systemSceneName;
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
            // 卸载当前场景
            if (_hasScene)
            {
                yield return UnloadCurrentLevel();
            }

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

            var loadMode = sceneRequest.IsAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single;
            var operation = SceneManager.LoadSceneAsync(sceneName, loadMode);
            if (operation != null)
            {
                operation.allowSceneActivation = false;
                while (operation.progress < 0.9f)
                {
                    Debug.Log($"Scene loading progress: {operation.progress * 100}%");
                    yield return null;
                }

                // 2) 这里可以：播完淡出、等数据预热、等 Addressables 资源等
                yield return new WaitForSeconds(0.2f);
                operation.allowSceneActivation = true;
                yield return new WaitUntil(() => operation.isDone);
            }

            yield return null;
            _currentScene = SceneManager.GetSceneByName(sceneName);
            _hasScene = _currentScene.IsValid() && _currentScene.isLoaded;
            // Simulate loading delay
            GameManager.Instance.PerformTransition(sceneRequest.GameTransition);
        }

        private void TestProgress(float progress)
        {
            Debug.Log($"Loading progress: {progress * 100}%");
        }

        private IEnumerator UnloadCurrentLevel()
        {
            if (!_hasScene) yield break;
            if (!_currentScene.IsValid() || !_currentScene.isLoaded) yield break;

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