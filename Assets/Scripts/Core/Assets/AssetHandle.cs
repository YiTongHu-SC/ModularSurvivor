using System;
using UnityEngine;

namespace Core.Assets
{
    /// <summary>
    /// 资源句柄，封装加载状态和资源引用
    /// </summary>
    public class AssetHandle<T> : IDisposable where T : UnityEngine.Object
    {
        public AssetKey Key { get; }
        public T Asset { get; private set; }
        public AssetLoadState State { get; private set; }
        public string ErrorMessage { get; private set; }
        public string ScopeName { get; internal set; }
        
        private int _referenceCount;
        private readonly object _lockObject = new object();
        
        public bool IsValid => State == AssetLoadState.Completed && Asset != null;
        public int ReferenceCount => _referenceCount;
        
        internal AssetHandle(AssetKey key, string scopeName = null)
        {
            Key = key;
            ScopeName = scopeName;
            State = AssetLoadState.Loading;
            _referenceCount = 1;
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
                _referenceCount++;
            }
        }
        
        internal int RemoveReference()
        {
            lock (_lockObject)
            {
                _referenceCount = Mathf.Max(0, _referenceCount - 1);
                return _referenceCount;
            }
        }
        
        public void Dispose()
        {
            Asset = null;
            State = AssetLoadState.Released;
        }
    }
    
    /// <summary>
    /// 资源加载状态
    /// </summary>
    public enum AssetLoadState
    {
        Loading,
        Completed,
        Failed,
        Released
    }
}
