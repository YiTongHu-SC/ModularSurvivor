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
        private Dictionary<GameTransition, string> _transitions;

        public void Initialize(List<LoadSceneMap> sceneMaps)
        {
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

            // Simulate loading delay
            GameManager.Instance.PerformTransition(sceneRequest.GameTransition);
        }

        private void TestProgress(float progress)
        {
            Debug.Log($"Loading progress: {progress * 100}%");
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