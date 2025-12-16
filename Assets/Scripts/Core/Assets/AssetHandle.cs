using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Assets
{
    /// <summary>
    ///     资源句柄，封装加载状态和资源引用
    /// </summary>
    public class AssetHandle<T> : IDisposable where T : Object
    {
        private readonly object _lockObject = new();

        internal AssetHandle(AssetKey key, string scopeName = null)
        {
            Key = key;
            ScopeName = scopeName;
            State = AssetLoadState.Loading;
            ReferenceCount = 1;
        }

        public AssetKey Key { get; }
        public T Asset { get; private set; }
        public AssetLoadState State { get; private set; }
        public string ErrorMessage { get; private set; }
        public string ScopeName { get; internal set; }

        public bool IsValid => State == AssetLoadState.Completed && Asset != null;
        public int ReferenceCount { get; private set; }

        public void Dispose()
        {
            Asset = null;
            State = AssetLoadState.Released;
        }

        internal void SetCompleted(T asset)
        {
            lock (_lockObject)
            {
                Asset = asset;
                State = AssetLoadState.Completed;
                ErrorMessage = null;
            }
        }

        internal void SetFailed(string errorMessage)
        {
            lock (_lockObject)
            {
                State = AssetLoadState.Failed;
                ErrorMessage = errorMessage;
                Asset = null;
            }
        }

        internal void AddReference()
        {
            lock (_lockObject)
            {
                ReferenceCount++;
            }
        }

        internal int RemoveReference()
        {
            lock (_lockObject)
            {
                ReferenceCount = Mathf.Max(0, ReferenceCount - 1);
                return ReferenceCount;
            }
        }
    }

    /// <summary>
    ///     资源加载状态
    /// </summary>
    public enum AssetLoadState
    {
        Loading,
        Completed,
        Failed,
        Released
    }
}