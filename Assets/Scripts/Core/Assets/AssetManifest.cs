using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Assets
{
    /// <summary>
    /// 资源清单，定义需要加载的资源列表
    /// </summary>
    [CreateAssetMenu(fileName = "AssetManifest", menuName = "Modular Survivor/Asset Manifest")]
    public class AssetManifest : ScriptableObject
    {
        [SerializeField] private ManifestEntry[] _entries;
        [SerializeField] private LoadFailureStrategy _failureStrategy = LoadFailureStrategy.LogWarning;
        
        public ManifestEntry[] Entries => _entries;
        public LoadFailureStrategy FailureStrategy => _failureStrategy;
        
        /// <summary>
        /// 获取所有资源键
        /// </summary>
        public AssetKey[] GetAllKeys()
        {
            var keys = new AssetKey[_entries.Length];
            for (int i = 0; i < _entries.Length; i++)
            {
                keys[i] = new AssetKey(_entries[i].Key);
            }
            return keys;
        }
        
        /// <summary>
        /// 按标签过滤资源键
        /// </summary>
        public AssetKey[] GetKeysByTag(string tag)
        {
            var keys = new List<AssetKey>();
            foreach (var entry in _entries)
            {
                if (Array.Exists(entry.Tags, t => t == tag))
                {
                    keys.Add(new AssetKey(entry.Key));
                }
            }
            return keys.ToArray();
        }
        
        /// <summary>
        /// 获取必需资源键
        /// </summary>
        public AssetKey[] GetRequiredKeys()
        {
            var keys = new List<AssetKey>();
            foreach (var entry in _entries)
            {
                if (entry.IsRequired)
                {
                    keys.Add(new AssetKey(entry.Key));
                }
            }
            return keys.ToArray();
        }
        
        /// <summary>
        /// 获取可选资源键
        /// </summary>
        public AssetKey[] GetOptionalKeys()
        {
            var keys = new List<AssetKey>();
            foreach (var entry in _entries)
            {
                if (!entry.IsRequired)
                {
                    keys.Add(new AssetKey(entry.Key));
                }
            }
            return keys.ToArray();
        }
        
        /// <summary>
        /// 计算总权重
        /// </summary>
        public float GetTotalWeight()
        {
            float total = 0f;
            foreach (var entry in _entries)
            {
                total += entry.Weight;
            }
            return total;
        }
    }
    
    /// <summary>
    /// 清单条目
    /// </summary>
    [Serializable]
    public class ManifestEntry
    {
        [SerializeField] private string _key;
        [SerializeField] private AssetType _assetType;
        [SerializeField] private float _weight = 1f;
        [SerializeField] private bool _isRequired = true;
        [SerializeField] private string[] _tags;
        
        public string Key => _key;
        public AssetType AssetType => _assetType;
        public float Weight => _weight;
        public bool IsRequired => _isRequired;
        public string[] Tags => _tags ?? Array.Empty<string>();
        
        public ManifestEntry(string key, AssetType assetType, float weight = 1f, bool isRequired = true, string[] tags = null)
        {
            _key = key;
            _assetType = assetType;
            _weight = weight;
            _isRequired = isRequired;
            _tags = tags;
        }
    }
    
    /// <summary>
    /// 加载失败策略
    /// </summary>
    public enum LoadFailureStrategy
    {
        Ignore,        // 忽略失败
        LogWarning,    // 记录警告
        LogError,      // 记录错误
        ThrowException // 抛出异常
    }
}
