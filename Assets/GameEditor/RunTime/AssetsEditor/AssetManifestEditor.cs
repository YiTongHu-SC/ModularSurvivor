using System;
using System.Linq;
using Core.Assets;
using Core.AssetsTool;
using UnityEditor;
using UnityEngine;

namespace GameEditor.RunTime.AssetsEditor
{
    [CustomEditor(typeof(AssetManifest))]
    public class AssetManifestEditor : Editor
    {
        private const string GlobalCatalogPath = "Assets/Configs/AssetsConfig/AssetCatalogGlobal.asset";
        private AssetManifest Manifest => (AssetManifest)target;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();
            if (GUILayout.Button("打印资源清单摘要")) LogManifestSummary(); // Quick manifest sanity check button.
            if (GUILayout.Button("自动填充资源条目")) AutoFillManifestEntries();
        }

        /// <summary>
        /// 根据生成规则自动填充资源条目
        /// </summary>
        private void AutoFillManifestEntries()
        {
            var manifest = Manifest;
            if (manifest == null)
            {
                Debug.LogWarning("AssetManifest target is missing.");
                return;
            }

            var catalog = AssetDatabase.LoadAssetAtPath<AssetCatalog>(GlobalCatalogPath);
            if (catalog == null)
            {
                Debug.LogError($"无法加载全局资源目录，路径: {GlobalCatalogPath}");
                return;
            }

            var generatedEntries = AssetManifestGenerator.GenerateEntriesFromCatalog(catalog);
            Undo.RecordObject(manifest, "Auto Fill Manifest Entries");
            manifest.SetEntries(generatedEntries);
            EditorUtility.SetDirty(manifest);
            Debug.Log($"已自动填充资源清单 '{manifest.name}' 的条目，共 {generatedEntries.Length} 条。", manifest);
        }

        private void LogManifestSummary()
        {
            var manifest = Manifest;
            if (manifest == null)
            {
                Debug.LogWarning("AssetManifest target is missing.");
                return;
            }

            var entries = manifest.Entries ?? Array.Empty<ManifestEntry>();
            var requiredCount = entries.Count(e => e != null && e.IsRequired);
            var optionalCount = entries.Count(e => e != null && !e.IsRequired);
            var duplicates = entries.Where(e => e != null && !string.IsNullOrEmpty(e.Key))
                .GroupBy(e => e.Key)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToArray();

            Debug.Log(
                $"AssetManifest '{manifest.name}' 总计 {entries.Length} 条目，必需 {requiredCount}，可选 {optionalCount}，总权重 {manifest.GetTotalWeight():0.##}",
                manifest);

            if (duplicates.Length > 0)
                Debug.LogWarning($"发现重复的资源键: {string.Join(", ", duplicates)}", manifest);
            else
                Debug.Log("未发现重复的资源键。", manifest);
        }
    }

    internal static class AssetManifestGenerator
    {
        public static ManifestEntry[] GenerateEntriesFromCatalog(AssetCatalog catalog)
        {
            throw new NotImplementedException();
        }
    }
}