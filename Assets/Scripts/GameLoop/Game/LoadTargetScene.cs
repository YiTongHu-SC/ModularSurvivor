using GameLoop.Config;
using UnityEngine;

namespace GameLoop.Game
{
    /// <summary>
    /// 游戏初始化，加载对应场景
    /// </summary>
    public class LoadTargetScene : MonoBehaviour
    {
        public LoadSceneConfig LoadSceneConfig;

        private void Start()
        {
            if (LoadSceneConfig == null)
            {
                Debug.LogError("GameInitConfig is null!");
                return;
            }

            Debug.Log($"Game Version: {LoadSceneConfig.Version}");
            Debug.Log($"Loading Scene: {LoadSceneConfig.LoadSceneName}");
            UnityEngine.SceneManagement.SceneManager.LoadScene(LoadSceneConfig.LoadSceneName);
        }
    }
}