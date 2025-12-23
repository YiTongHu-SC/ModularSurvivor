using Core.Assets;
using Core.AssetsTool;
using GameEditor.Editor.AssetsEditor;
using GameEditor.Editor.Config;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
using ObjectField = UnityEditor.UIElements.ObjectField;

namespace GameEditor.Editor.Tools
{
    public class AssetsToolEditor : EditorWindow
    {
        private const string UxmlPath = "Assets/GameEditor/EditorAssets/UI/AssetsToolWindow.uxml";
        private const string UssPath = "Assets/GameEditor/EditorAssets/UI/AssetsToolUss.uss";
        private const string LastSelectedAssetPrefsKey = "AssetsToolEditor_LastSelectedAsset";

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

            // 加载 USS
            var uss = AssetDatabase.LoadAssetAtPath<StyleSheet>(UssPath);
            if (uss != null)
                rootVisualElement.styleSheets.Add(uss);

            // 查找控件
            var helloButton = rootVisualElement.Q<Button>("helloButton");
            var resultLabel = rootVisualElement.Q<Label>("resultLabel");
            var objField = rootVisualElement.Q<ObjectField>("configAsset");
            // 设置初始状态
            if (resultLabel != null)
                resultLabel.text = "Ready.";

            // 绑定交互逻辑
            if (helloButton != null)
            {
                helloButton.clicked += PerformActionAutoLoadAssetsCatalog;
            }

            if (objField != null)
            {
                objField.objectType = typeof(ScriptableObject);

                // 自动加载上次选择的资产
                LoadLastSelectedAsset(objField, resultLabel);

                objField.RegisterValueChangedCallback(evt =>
                {
                    var selectedObject = evt.newValue as AssetsToolEditorConfig;
                    if (resultLabel != null)
                    {
                        resultLabel.text = selectedObject != null
                            ? $"Selected: {selectedObject.name}"
                            : "No asset selected.";
                    }

                    // 保存选择的资产路径
                    SaveSelectedAssetPath(selectedObject);
                });
            }
        }

        private void PerformActionAutoLoadAssetsCatalog()
        {
            Debug.Log("Performing action...");
            // Add your action logic here
            string savedPath = EditorPrefs.GetString(LastSelectedAssetPrefsKey);
            if (string.IsNullOrEmpty(savedPath))
            {
                Debug.LogError($"No asset path provided for {LastSelectedAssetPrefsKey}");
            }

            var config = AssetDatabase.LoadAssetAtPath<AssetsToolEditorConfig>(savedPath);
            Debug.Log("Loaded Asset Tool Config: " + (config != null ? config.name : "null"));
            var assetCatalog = config.AssetCatalogFile;
            Debug.Log("Loaded Asset Catalog: " + (assetCatalog != null ? assetCatalog.name : "null"));
            // Further processing 
            // 递归遍历资产目录下所有资源
            var targetDir = config.AssetTargetPath;
            string[] allAssetGuids = AssetDatabase.FindAssets("", new[] { targetDir });
            Debug.Log($"Found {allAssetGuids.Length} assets in directory: {targetDir}");
            assetCatalog.Clear();

            // 先过滤掉文件夹，只保留实际的文件
            var validAssets = new System.Collections.Generic.List<string>();
            foreach (string guid in allAssetGuids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                // 跳过文件夹 - 检查是否为目录
                if (System.IO.Directory.Exists(assetPath))
                {
                    Debug.Log($"Skipping directory: {assetPath}");
                    continue;
                }

                validAssets.Add(guid);
            }

            Debug.Log(
                $"Processing {validAssets.Count} files (skipped {allAssetGuids.Length - validAssets.Count} directories)");
            assetCatalog.SafeResetEntriesMax(validAssets.Count);

            var counter = 0;
            foreach (string guid in validAssets)
            {
                // 加载资源并进行处理
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                Debug.Log($"Load Asset Path: {assetPath}");
                var entry = new AssetCatalogEntry(
                    AssetToolExtensions.GetAssetPathKey(assetPath),
                    AssetToolExtensions.GetAssetPathResource(assetPath),
                    AssetToolExtensions.GetAssetType(asset));
                assetCatalog.TrySetEntryAt(counter, entry);
                counter++;
            }

            EditorUtility.SetDirty(assetCatalog);
            Debug.Log($"Asset Catalog '{assetCatalog.name}' updated with {counter} entries.");
            Debug.Log($"Asset Catalog Update Complete.");
        }

        /// <summary>
        /// 保存选择的资产路径到 EditorPrefs
        /// </summary>
        private void SaveSelectedAssetPath(AssetsToolEditorConfig selectedAsset)
        {
            if (selectedAsset != null)
            {
                string assetPath = AssetDatabase.GetAssetPath(selectedAsset);
                EditorPrefs.SetString(LastSelectedAssetPrefsKey, assetPath);
            }
            else
            {
                EditorPrefs.DeleteKey(LastSelectedAssetPrefsKey);
            }
        }

        /// <summary>
        /// 从 EditorPrefs 加载上次选择的资产
        /// </summary>
        private void LoadLastSelectedAsset(ObjectField objField, Label resultLabel)
        {
            if (EditorPrefs.HasKey(LastSelectedAssetPrefsKey))
            {
                string savedPath = EditorPrefs.GetString(LastSelectedAssetPrefsKey);
                if (!string.IsNullOrEmpty(savedPath))
                {
                    var savedAsset = AssetDatabase.LoadAssetAtPath<AssetsToolEditorConfig>(savedPath);
                    if (savedAsset != null)
                    {
                        objField.value = savedAsset;
                        if (resultLabel != null)
                        {
                            resultLabel.text = $"Auto-loaded: {savedAsset.name}";
                        }
                    }
                    else
                    {
                        // 如果资产不存在了，清除已保存的路径
                        EditorPrefs.DeleteKey(LastSelectedAssetPrefsKey);
                        if (resultLabel != null)
                        {
                            resultLabel.text = "Previously saved asset not found.";
                        }
                    }
                }
            }
        }
    }
}