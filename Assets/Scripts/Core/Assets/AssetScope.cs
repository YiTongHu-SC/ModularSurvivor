using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Assets
{
    /// <summary>
    ///     资源作用域，管理一组资源的生命周期
    /// </summary>
    public class AssetScope : IDisposable
    {
        private readonly List<object> _handles;
        private readonly object _lockObject = new();

        private readonly IAssetProvider _provider;

        internal AssetScope(AssetsScopeLabel label, IAssetProvider provider)
        {
            ScopeLabel = label;
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _handles = new List<object>();
        }

        public AssetsScopeLabel ScopeLabel { get; }

        public int HandleCount
        {
            get
            {
                lock (_lockObject)
                {
                    return _handles.Count;
                }
            }
        }

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            if (IsDisposed) return;

            ReleaseAll();
            IsDisposed = true;
        }

        /// <summary>
        ///     在此作用域中获取资源
        /// </summary>
        public AssetHandle<T> Acquire<T>(AssetKey key) where T : Object
        {
            ThrowIfDisposed();

            var handle = _provider.Load<T>(key, ScopeLabel);

            lock (_lockObject)
            {
                _handles.Add(handle);
            }

            return handle;
        }

        /// <summary>
        ///     在此作用域中异步获取资源
        /// </summary>
        public async Task<AssetHandle<T>> AcquireAsync<T>(AssetKey key)
            where T : Object
        {
            ThrowIfDisposed();

            var handle = await _provider.LoadAsync<T>(key, ScopeLabel);

            lock (_lockObject)
            {
                _handles.Add(handle);
            }

            return handle;
        }

        /// <summary>
        ///     批量获取资源
        /// </summary>
        public async Task<AssetHandle<T>[]> AcquireBatch<T>(AssetKey[] keys,
            IProgress<float> progress = null) where T : Object
        {
            ThrowIfDisposed();

            var handles = await _provider.LoadBatchAsync<T>(keys, ScopeLabel, progress);

            lock (_lockObject)
            {
                foreach (var handle in handles) _handles.Add(handle);
            }

            return handles;
        }

        /// <summary>
        ///     实例化预制体
        /// </summary>
        public async Task<GameObject> InstantiateAsync(AssetKey key, Transform parent = null)
        {
            ThrowIfDisposed();
            return await _provider.InstantiateAsync(key, parent, ScopeLabel);
        }

        /// <summary>
        ///     释放所有资源
        /// </summary>
        public void ReleaseAll()
        {
            lock (_lockObject)
            {
                foreach (var handle in _handles) ReleaseHandle(handle);

                _handles.Clear();
            }
        }

        private void ReleaseHandle(object handle)
        {
            // 使用反射释放不同类型的句柄
            var handleType = handle.GetType();
            if (handleType.IsGenericType && handleType.GetGenericTypeDefinition() == typeof(AssetHandle<>))
            {
                var releaseMethod = typeof(IAssetProvider).GetMethod(nameof(IAssetProvider.Release));
                if (releaseMethod != null)
                {
                    var genericMethod = releaseMethod.MakeGenericMethod(handleType.GetGenericArguments());
                    genericMethod.Invoke(_provider, new[] { handle });
                }
            }
        }

        private void ThrowIfDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(AssetScope));
        }
    }
}