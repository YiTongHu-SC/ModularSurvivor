using UnityEngine;
using UI.Framework;

namespace UI.Framework
{
    /// <summary>
    /// MVC框架测试场景 - 快速设置和演示框架功能
    /// </summary>
    public class MVCTestScene : MonoBehaviour
    {
        [Header("Test Scene Settings")]
        [SerializeField] private bool autoSetup = true;
        [SerializeField] private bool showGUI = true;
        
        private MVCManager _mvcManager;
        private MVCFrameworkUsageExample _usageExample;

        private void Start()
        {
            if (autoSetup)
            {
                SetupMVCFramework();
            }
        }

        /// <summary>
        /// 设置MVC框架
        /// </summary>
        public void SetupMVCFramework()
        {
            // 创建MVCManager如果不存在
            if (MVCManager.Instance == null)
            {
                var mvcManagerGO = new GameObject("MVCManager");
                _mvcManager = mvcManagerGO.AddComponent<MVCManager>();
                DontDestroyOnLoad(mvcManagerGO);
            }
            else
            {
                _mvcManager = MVCManager.Instance;
            }

            // 创建使用示例组件
            var exampleGO = new GameObject("MVCUsageExample");
            _usageExample = exampleGO.AddComponent<MVCFrameworkUsageExample>();
            exampleGO.transform.SetParent(transform);

            Debug.Log("MVC Framework setup complete!");
            Debug.Log("Press M to open Main Menu, S for Settings, ESC to close top UI, I for info");
        }

        private void OnGUI()
        {
            if (!showGUI) return;

            GUILayout.BeginArea(new Rect(10, 10, 300, 400));
            GUILayout.BeginVertical("box");
            
            GUILayout.Label("MVC Framework Test Panel");
            GUILayout.Space(10);

            if (GUILayout.Button("Open Main Menu"))
            {
                _usageExample?.OpenMainMenu();
            }

            if (GUILayout.Button("Open Settings"))
            {
                _usageExample?.OpenSettings();
            }

            if (GUILayout.Button("Close Top UI"))
            {
                _usageExample?.CloseTopUI();
            }

            if (GUILayout.Button("Show Stack Info"))
            {
                _usageExample?.ShowStackInfo();
            }

            if (GUILayout.Button("Close All UI"))
            {
                _usageExample?.CloseAllUI();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Test Input Blocking"))
            {
                _usageExample?.TestInputBlocking();
            }

            if (GUILayout.Button("Test Stack Behavior"))
            {
                _usageExample?.TestStackBehavior();
            }

            GUILayout.Space(10);

            if (_mvcManager != null)
            {
                GUILayout.Label($"UI Controllers: {_mvcManager.GetStatistics().Split('\n')[2]}");
                GUILayout.Label($"Has Input Blocker: {_mvcManager.HasInputBlocker()}");
                
                if (_mvcManager.HasInputBlocker())
                {
                    var originalColor = GUI.color;
                    GUI.color = Color.red;
                    GUILayout.Label("INPUT BLOCKED");
                    GUI.color = originalColor;
                }
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}
