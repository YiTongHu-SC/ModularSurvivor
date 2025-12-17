using System;
using UnityEngine;

namespace Core.Assets
{
    /// <summary>
    ///     资源键，业务层只使用此键，不直接使用路径
    /// </summary>
    [Serializable]
    public struct AssetKey : IEquatable<AssetKey>
    {
        [SerializeField] private string _key;

        public string Key => _key;

        public AssetKey(string key)
        {
            _key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public static implicit operator AssetKey(string key)
        {
            return new AssetKey(key);
        }

        public static implicit operator string(AssetKey assetKey)
        {
            return assetKey._key;
        }

        public bool Equals(AssetKey other)
        {
            return _key == other._key;
        }

        public override bool Equals(object obj)
        {
            return obj is AssetKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _key?.GetHashCode() ?? 0;
        }

        public override string ToString()
        {
            return _key ?? "null";
        }

        public static bool operator ==(AssetKey left, AssetKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AssetKey left, AssetKey right)
        {
            return !left.Equals(right);
        }

        public bool IsValid => !string.IsNullOrEmpty(_key);
    }
}