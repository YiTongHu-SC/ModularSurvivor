using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Assets
{
    /// <summary>
    /// 资源目录，管理 AssetKey 到路径的映射关系
    /// </summary>
    [CreateAssetMenu(fileName = "AssetCatalog", menuName = "Modular Survivor/Asset Catalog")]
    public class AssetCatalog : ScriptableObject
    {
        [SerializeField] private AssetCatalogEntry[] _entries;
        
        private Dictionary<string, AssetCatalogEntry> _entryMap;
        
        private void OnEnable()
        {
            BuildEntryMap();
        }
        
        private void BuildEntryMap()
        {
            _entryMap = new Dictionary<string, AssetCatalogEntry>();
            
            if (_entries == null) return;
            
            foreach (var entry in _entries)
            {
                if (string.IsNullOrEmpty(entry.Key))
                {
                    Debug.LogWarning($"Empty key found in asset catalog: {name}");
                    continue;
                }
                
                if (_entryMap.ContainsKey(entry.Key))
                {
                    Debug.LogWarning($"Duplicate key found in asset catalog: {entry.Key}");
                    continue;
                }
                
                _entryMap[entry.Key] = entry;
            }
        }
        
        /// <summary>
        /// 获取资源路径
        /// </summary>
        public bool TryGetResourcesPath(AssetKey key, out string resourcesPath)
        {
            if (_entryMap == null)
            {
                BuildEntryMap();
            }
            
            if (_entryMap != null && _entryMap.TryGetValue(key.Key, out var entry))
            {
                resourcesPath = entry.ResourcesPath;
                return true;
            }
            
            resourcesPath = null;
            return false;
        }
        
        /// <summary>
        /// 获取资源条目
        /// </summary>
        public bool TryGetEntry(AssetKey key, out AssetCatalogEntry entry)
        {
            if (_entryMap == null)
            {
                BuildEntryMap();
            }
            
            entry = null;
            return _entryMap != null && _entryMap.TryGetValue(key.Key, out entry);
        }
        
        /// <summary>
        /// 根据标签获取所有键
        /// </summary>
        public AssetKey[] GetKeysByTag(string tag)
        {
            if (_entryMap == null)
            {
                BuildEntryMap();
            }
            
            if (_entryMap == null)
                return Array.Empty<AssetKey>();
            
            return _entryMap.Values
                .Where(entry => entry.Tags.Contains(tag))
                .Select(entry => new AssetKey(entry.Key))
                .ToArray();
        }
        
        /// <summary>
        /// 获取所有键
        /// </summary>
        public AssetKey[] GetAllKeys()
        {
            if (_entryMap == null)
            {
                BuildEntryMap();
            }
            
            if (_entryMap == null)
                return Array.Empty<AssetKey>();
            
            return _entryMap.Keys.Select(key => new AssetKey(key)).ToArray();
        }
        
        public AssetCatalogEntry[] Entries => _entries;
    }
}
