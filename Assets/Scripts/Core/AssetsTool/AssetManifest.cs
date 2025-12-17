using System;
using System.Collections.Generic;
using Core.Assets;
using UnityEngine;

namespace Core.AssetsTool
{
    /// <summary>
    ///     资源清单，定义需要加载的资源列表
    /// </summary>
    [CreateAssetMenu(fileName = "AssetManifest", menuName = "Assets Config/Asset Manifest")]
    public class AssetManifest : ScriptableObject
    {
        public AssetCatalog Catalog;

        public List<ManifestRule> Rules = new();

        // 可选：手工附加或排除（解决规则不好表达的例外）
        public List<ManifestOverride> Overrides = new();
        [SerializeField] private List<ManifestEntry> _generatedEntries = new();
        [SerializeField] private LoadFailureStrategy _failureStrategy = LoadFailureStrategy.LogWarning;
        public IReadOnlyList<ManifestEntry> Entries => _generatedEntries;
        public LoadFailureStrategy FailureStrategy => _failureStrategy;

        /// <summary>
        ///     获取所有资源键
        /// </summary>
        public AssetKey[] GetAllKeys()
        {
            var keys = new AssetKey[_generatedEntries.Count];
            for (var i = 0; i < _generatedEntries.Count; i++)
            {
                keys[i] = new AssetKey(_generatedEntries[i].Key);
            }

            return keys;
        }

        /// <summary>
        ///     按标签过滤资源键
        /// </summary>
        public AssetKey[] GetKeysByTag(string tag)
        {
            var keys = new List<AssetKey>();
            foreach (var entry in _generatedEntries)
            {
                if (Array.Exists(entry.Tags, t => t == tag))
                {
                    keys.Add(new AssetKey(entry.Key));
                }
            }

            return keys.ToArray();
        }

        /// <summary>
        ///     获取必需资源键
        /// </summary>
        public AssetKey[] GetRequiredKeys()
        {
            var keys = new List<AssetKey>();
            foreach (var entry in _generatedEntries)
            {
                if (entry.IsRequired)
                {
                    keys.Add(new AssetKey(entry.Key));
                }
            }

            return keys.ToArray();
        }

        /// <summary>
        ///     获取可选资源键
        /// </summary>
        public AssetKey[] GetOptionalKeys()
        {
            var keys = new List<AssetKey>();
            foreach (var entry in _generatedEntries)
                if (!entry.IsRequired)
                    keys.Add(new AssetKey(entry.Key));

            return keys.ToArray();
        }

        /// <summary>
        ///     计算总权重
        /// </summary>
        public float GetTotalWeight()
        {
            var total = 0f;
            foreach (var entry in _generatedEntries) total += entry.Weight;

            return total;
        }

        public void SetEntries(List<ManifestEntry> entries)
        {
            _generatedEntries.Clear();
            _generatedEntries.AddRange(entries);
        }
    }

    /// <summary>
    ///     清单条目
    /// </summary>
    [Serializable]
    public class ManifestEntry
    {
        [SerializeField] private string _key;
        [SerializeField] private AssetType _assetType;
        [SerializeField] private float _weight = 1f;
        [SerializeField] private bool _isRequired = true;
        [SerializeField] private string[] _tags;

        public ManifestEntry(string key, AssetType assetType, float weight = 1f, bool isRequired = true,
            string[] tags = null)
        {
            _key = key;
            _assetType = assetType;
            _weight = weight;
            _isRequired = isRequired;
            _tags = tags;
        }

        public string Key => _key;
        public AssetType AssetType => _assetType;

        public float Weight
        {
            get => _weight;
            set => _weight = value;
        }

        public bool IsRequired
        {
            get => _isRequired;
            set => _isRequired = value;
        }

        public string[] Tags => _tags ?? Array.Empty<string>();
    }

    /// <summary>
    ///     加载失败策略
    /// </summary>
    public enum LoadFailureStrategy
    {
        Ignore, // 忽略失败
        LogWarning, // 记录警告
        LogError, // 记录错误
        ThrowException // 抛出异常
    }

    [Serializable]
    public class ManifestRule
    {
        public string Name = "Rule";

        public bool Enabled = true;

        // 过滤条件
        public List<string> IncludeKeyPrefixes = new();
        public List<string> ExcludeKeyPrefixes = new();
        public List<string> IncludeTags = new();
        public List<string> ExcludeTags = new();
        public List<string> IncludeTypes = new(); // 用 string 存 "GameObject" 等
        public List<string> ExcludeTypes = new();

        // 输出策略
        public bool DefaultRequired = false;
        public List<string> RequiredTags = new() { "required" };

        // 最简权重策略：按 tag / type 给权重
        public List<TagWeight> TagWeights = new();
        public List<TypeWeight> TypeWeights = new();
        public float DefaultWeight = 1f;
    }

    [Serializable]
    public struct TagWeight
    {
        public string Tag;
        public float Weight;
    }

    [Serializable]
    public struct TypeWeight
    {
        public string type;
        public float weight;
    }

    [Serializable]
    public class ManifestOverride
    {
        public string key;
        public bool exclude; // true：强制排除
        public bool requiredOverride; // 是否覆盖 required
        public bool requiredValue;
        public bool weightOverride; // 是否覆盖 weight
        public float weightValue;
    }
}