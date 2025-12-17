using System;
using UnityEngine;

namespace Core.Assets
{
    /// <summary>
    ///     资源目录条目，用于 AssetKey 到实际路径的映射
    /// </summary>
    [Serializable]
    public class AssetCatalogEntry
    {
        [SerializeField] private string _key;
        [SerializeField] private string _resourcesPath;
        [SerializeField] private AssetType _assetType;
        [SerializeField] private string[] _tags;

        public AssetCatalogEntry(string key, string resourcesPath, AssetType assetType, string[] tags = null)
        {
            _key = key;
            _resourcesPath = resourcesPath;
            _assetType = assetType;
            _tags = tags ?? Array.Empty<string>();
        }

        public string Key => _key;
        public string ResourcesPath => _resourcesPath;
        public AssetType AssetType => _assetType;
        public string[] Tags => _tags;
    }

    /// <summary>
    ///     资源类型枚举
    /// </summary>
    public enum AssetType
    {
        Prefab,
        ScriptableObject,
        Sprite,
        AudioClip,
        TextAsset,
        Material,
        Shader,
        Texture,
        Other
    }
}