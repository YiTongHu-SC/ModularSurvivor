using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameEditor.RunTime.Tools
{
    public class AssetsToolEditor : EditorWindow
    {
        private const string UxmlPath = "Assets/GameEditor/EditorAssets/UI/AssetsToolWindow.uxml";
        private const string UssPath = "Assets/GameEditor/EditorAssets/UI/AssetsToolUss.uss";

        [MenuItem("Tools/Assets Tool")]
        public static void ShowWindow()
        {
            var wnd = GetWindow<AssetsToolEditor>();
            wnd.titleContent = new GUIContent("AssetsToolEditor");
            wnd.minSize = new Vector2(320, 180);
        }

        public void CreateGUI()
        {
            // 1) 清空根节点（防止重载/Domain Reload 造成重复 UI）
            rootVisualElement.Clear();

            // 2) 加载 UXML 并实例化到窗口根节点
            var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath);
            if (uxml == null)
            {
                rootVisualElement.Add(new Label($"UXML not found: {UxmlPath}"));
                return;
            }

            // 给根节点一个 class，方便 USS 写样式
            rootVisualElement.AddToClassList("root");
            uxml.CloneTree(rootVisualElement);

            // 3) 加载 USS
            var uss = AssetDatabase.LoadAssetAtPath<StyleSheet>(UssPath);
            if (uss != null)
                rootVisualElement.styleSheets.Add(uss);

            // 4) 查找控件
            var nameField = rootVisualElement.Q<TextField>("nameField");
            var helloButton = rootVisualElement.Q<Button>("helloButton");
            var resultLabel = rootVisualElement.Q<Label>("resultLabel");

            // 5) 设置初始状态
            if (resultLabel != null)
                resultLabel.text = "Ready.";

            // 6) 绑定交互逻辑
            if (helloButton != null)
            {
                helloButton.clicked += () =>
                {
                    var name = nameField?.value;
                    if (string.IsNullOrEmpty(name))
                        name = "World";

                    if (resultLabel != null)
                        resultLabel.text = $"Hello, {name}!";
                };
            }
        }
    }
}