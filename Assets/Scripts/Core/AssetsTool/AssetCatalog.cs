using System;
using System.Collections.Generic;
using System.Linq;
using Core.Assets;
using UnityEngine;

namespace Core.AssetsTool
{
    /// <summary>
    ///     资源目录，管理 AssetKey 到路径的映射关系
    /// </summary>
    [CreateAssetMenu(fileName = "AssetCatalog", menuName = "Assets Config/Asset Catalog")]
    public class AssetCatalog : ScriptableObject
    {
        [SerializeField] private AssetCatalogEntry[] EntriesConfig;

        private Dictionary<string, AssetCatalogEntry> _entryMap;

        public AssetCatalogEntry[] Entries => EntriesConfig;

        private void OnEnable()
        {
            BuildEntryMap();
        }

        public void Clear()
        {
            EntriesConfig = null;
            _entryMap = null;
        }

        public void BuildEntryMap()
        {
            _entryMap = new Dictionary<string, AssetCatalogEntry>();

            if (EntriesConfig == null) return;

            foreach (var entry in EntriesConfig)
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
        ///     获取资源路径
        /// </summary>
        public bool TryGetResourcesPath(AssetKey key, out string resourcesPath)
        {
            if (_entryMap == null) BuildEntryMap();

            if (_entryMap != null && _entryMap.TryGetValue(key.Key, out var entry))
            {
                resourcesPath = entry.ResourcesPath;
                return true;
            }

            resourcesPath = null;
            return false;
        }

        public void SafeResetEntriesMax(int maxEntries)
        {
            if (maxEntries < 0)
            {
                Debug.LogWarning("Max entries cannot be negative.");
                return;
            }

            var oldEntries = EntriesConfig ?? Array.Empty<AssetCatalogEntry>();
            var newEntries = new AssetCatalogEntry[maxEntries];
            Array.Copy(oldEntries, newEntries, Math.Min(oldEntries.Length, maxEntries));
            EntriesConfig = newEntries;
        }

        public void TrySetEntryAt(int index, AssetCatalogEntry newEntry)
        {
            EntriesConfig ??= new AssetCatalogEntry[] { newEntry };

            if (index < 0)
            {
                Debug.LogWarning($"Index {index} is out of bounds for EntriesConfig.");
                return;
            }

            if (index >= EntriesConfig.Length)
            {
                var oldEntries = EntriesConfig;
                EntriesConfig = new AssetCatalogEntry[index + 1];
                Array.Copy(oldEntries, EntriesConfig, oldEntries.Length);
            }

            EntriesConfig[index] = newEntry;
        }

        /// <summary>
        ///     获取资源条目
        /// </summary>
        public bool TryGetEntry(AssetKey key, out AssetCatalogEntry entry)
        {
            if (_entryMap == null) BuildEntryMap();

            entry = null;
            return _entryMap != null && _entryMap.TryGetValue(key.Key, out entry);
        }

        /// <summary>
        ///     根据标签获取所有键
        /// </summary>
        public AssetKey[] GetKeysByTag(string tag)
        {
            if (_entryMap == null) BuildEntryMap();

            if (_entryMap == null)
                return Array.Empty<AssetKey>();

            return _entryMap.Values
                .Where(entry => entry.Tags.Contains(tag))
                .Select(entry => new AssetKey(entry.Key))
                .ToArray();
        }

        /// <summary>
        ///     获取所有键
        /// </summary>
        public AssetKey[] GetAllKeys()
        {
            if (_entryMap == null) BuildEntryMap();

            if (_entryMap == null)
                return Array.Empty<AssetKey>();

            return _entryMap.Keys.Select(key => new AssetKey(key)).ToArray();
        }
    }
}