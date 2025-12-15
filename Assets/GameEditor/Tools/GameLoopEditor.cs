using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameEditor.Tools
{
    [InitializeOnLoad]
    public class GameLoopEditor : EditorWindow
    {
        private const string StartScenePath = "Assets/Scenes/GameInit.unity";
        private const string LastScenePathKey = "LastScenePathKey";

        static GameLoopEditor()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        /// <summary>
        /// 保证退出场景时，加载进入前的场景
        /// </summary>
        /// <param name="state"></param>
        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                // 当退出播放模式时，加载之前保存的场景
                if (EditorPrefs.HasKey(LastScenePathKey))
                {
                    string lastScenePath = EditorPrefs.GetString(LastScenePathKey);
                    if (!string.IsNullOrEmpty(lastScenePath) && System.IO.File.Exists(lastScenePath))
                    {
                        EditorSceneManager.OpenScene(lastScenePath);
                    }

                    EditorPrefs.DeleteKey(LastScenePathKey);
                }
            }
        }

        [MenuItem("Debug/RunGame")]
        private static void RunGame()
        {
            if (Application.isPlaying)
            {
                Debug.LogWarning("Game is already running.");
                return;
            }

            // 检查GameLoading场景是否存在
            if (System.IO.File.Exists(StartScenePath))
            {
                string currentScenePath = SceneManager.GetActiveScene().path;
                EditorPrefs.SetString(LastScenePathKey, currentScenePath);

                Debug.Log($"Load Scene: {StartScenePath}");
                // 打卡GameLoading场景
                EditorSceneManager.OpenScene(StartScenePath);
                // 运行游戏
                EditorApplication.EnterPlaymode();
            }
            else
            {
                Debug.LogError("GameLoading场景不存在: " + StartScenePath);
            }
        }
    }
}