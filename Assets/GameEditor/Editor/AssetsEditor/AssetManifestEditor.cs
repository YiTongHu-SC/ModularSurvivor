using System;
using System.Collections.Generic;
using System.Linq;
using Core.Assets;
using Core.AssetsTool;
using UnityEditor;
using UnityEngine;

namespace GameEditor.Editor.AssetsEditor
{
    [CustomEditor(typeof(AssetManifest))]
    public class AssetManifestEditor : UnityEditor.Editor
    {
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

            var catalog = manifest.Catalog;
            if (catalog == null)
            {
                Debug.LogError("AssetManifest 的 Catalog 未设置，无法生成条目。", manifest);
                return;
            }

            var generatedEntries = AssetManifestGenerator.Generate(catalog, manifest.Rules, manifest.Overrides);
            Undo.RecordObject(manifest, "Auto Fill Manifest Entries");
            manifest.SetEntries(generatedEntries);
            EditorUtility.SetDirty(manifest);
            Debug.Log($"已自动填充资源清单 '{manifest.name}' 的条目，共 {generatedEntries.Count} 条。", manifest);
        }

        private void LogManifestSummary()
        {
            var manifest = Manifest;
            if (manifest == null)
            {
                Debug.LogWarning("AssetManifest target is missing.");
                return;
            }

            var entries = manifest.Entries;
            var requiredCount = entries.Count(e => e != null && e.IsRequired);
            var optionalCount = entries.Count(e => e != null && !e.IsRequired);
            var duplicates = entries.Where(e => e != null && !string.IsNullOrEmpty(e.Key))
                .GroupBy(e => e.Key)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToArray();

            Debug.Log(
                $"AssetManifest '{manifest.name}' 总计 {entries.Count} 条目，必需 {requiredCount}，可选 {optionalCount}，总权重 {manifest.GetTotalWeight():0.##}",
                manifest);

            if (duplicates.Length > 0)
                Debug.LogWarning($"发现重复的资源键: {string.Join(", ", duplicates)}", manifest);
            else
                Debug.Log("未发现重复的资源键。", manifest);
        }
    }

    public static class AssetManifestGenerator
    {
        public static List<ManifestEntry> Generate(AssetCatalog catalog, List<ManifestRule> rules,
            List<ManifestOverride> overrides)
        {
            var result = new Dictionary<string, ManifestEntry>(StringComparer.Ordinal);

            foreach (var e in catalog.Entries)
            {
                foreach (var rule in rules)
                {
                    if (rule == null || !rule.Enabled) continue;
                    if (!PassRule(e, rule)) continue;

                    var entry = ToEntry(e, rule);

                    if (result.TryGetValue(entry.Key, out var existing))
                    {
                        existing.IsRequired |= entry.IsRequired;
                        existing.Weight = Mathf.Max(existing.Weight, entry.Weight);
                        result[entry.Key] = existing;
                    }
                    else
                    {
                        result.Add(entry.Key, entry);
                    }
                }
            }

            ApplyOverrides(result, catalog, overrides);

            // 可选：排序（按 key）
            return result.Values.OrderBy(x => x.Key, StringComparer.Ordinal).ToList();
        }

        static bool PassRule(AssetCatalogEntry e, ManifestRule r)
        {
            bool PassPrefix(List<string> prefixes)
            {
                if (prefixes == null || prefixes.Count == 0) return true;
                return prefixes.Any(p => !string.IsNullOrEmpty(p) && e.Key.StartsWith(p, StringComparison.Ordinal));
            }

            bool PassTags(List<string> tags)
            {
                if (tags == null || tags.Count == 0) return true;
                if (e.Tags == null) return false;
                return e.Tags.Intersect(tags).Any();
            }

            bool PassTypes(List<string> types)
            {
                if (types == null || types.Count == 0) return true;
                return types.Any(t => string.Equals(t, e.AssetType.ToString(), StringComparison.Ordinal));
            }

            bool HitPrefix(List<string> prefixes) => prefixes != null && prefixes.Any(p =>
                !string.IsNullOrEmpty(p) && e.Key.StartsWith(p, StringComparison.Ordinal));

            bool HitTags(List<string> tags) => tags != null && e.Tags != null && e.Tags.Intersect(tags).Any();

            bool HitTypes(List<string> types) =>
                types != null && types.Any(t => string.Equals(t, e.AssetType.ToString(), StringComparison.Ordinal));

            var includeOk = PassPrefix(r.IncludeKeyPrefixes) && PassTags(r.IncludeTags) && PassTypes(r.IncludeTypes);
            if (!includeOk) return false;

            var excluded = HitPrefix(r.ExcludeKeyPrefixes) || HitTags(r.ExcludeTags) || HitTypes(r.ExcludeTypes);
            return !excluded;
        }

        private static ManifestEntry ToEntry(AssetCatalogEntry e, ManifestRule r)
        {
            bool required = r.DefaultRequired || (e.Tags != null && e.Tags.Intersect(r.RequiredTags ?? new()).Any());
            float weight = ResolveWeight(e, r);

            return new ManifestEntry(e.Key, e.AssetType, weight, required, e.Tags);
        }

        private static float ResolveWeight(AssetCatalogEntry e, ManifestRule r)
        {
            float w = r.DefaultWeight;

            if (e.Tags != null && r.TagWeights != null)
            {
                foreach (var tw in r.TagWeights)
                    if (!string.IsNullOrEmpty(tw.Tag) && e.Tags.Contains(tw.Tag))
                        w = Mathf.Max(w, tw.Weight);
            }

            if (r.TypeWeights != null)
            {
                foreach (var tw in r.TypeWeights)
                    if (!string.IsNullOrEmpty(tw.type) &&
                        string.Equals(tw.type, e.AssetType.ToString(), StringComparison.Ordinal))
                        w = Mathf.Max(w, tw.weight);
            }

            return w;
        }

        static void ApplyOverrides(Dictionary<string, ManifestEntry> dict, AssetCatalog catalog,
            List<ManifestOverride> overrides)
        {
            if (overrides == null) return;

            foreach (var ov in overrides)
            {
                if (ov == null || string.IsNullOrEmpty(ov.key)) continue;

                if (ov.exclude)
                {
                    dict.Remove(ov.key);
                    continue;
                }

                if (!dict.TryGetValue(ov.key, out var entry))
                {
                    // 允许 override 添加（如果 catalog 有）
                    if (catalog.TryGetEntry(ov.key, out var catalogEntry))
                        entry = new ManifestEntry(catalogEntry.Key, catalogEntry.AssetType, 1f, false,
                            catalogEntry.Tags);
                    else
                        entry = new ManifestEntry(ov.key, catalogEntry.AssetType, 1f, false);
                }

                if (ov.requiredOverride) entry.IsRequired = ov.requiredValue;
                if (ov.weightOverride) entry.Weight = ov.weightValue;

                dict[ov.key] = entry;
            }
        }
    }
}