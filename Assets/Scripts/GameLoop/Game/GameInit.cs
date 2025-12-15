using GameLoop.Config;
using UnityEngine;

namespace GameLoop.Game
{
    /// <summary>
    /// 游戏初始化，加载对应场景
    /// </summary>
    public class GameInit : MonoBehaviour
    {
        public GameInitConfig GameInitConfig;

        private void Start()
        {
            if (GameInitConfig == null)
            {
                Debug.LogError("GameInitConfig is null!");
                return;
            }

            Debug.Log($"Game Version: {GameInitConfig.version}");
            UnityEngine.SceneManagement.SceneManager.LoadScene(GameInitConfig.LoadSceneName);
        }
    }
}