using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Assets
{
    /// <summary>
    ///     基于 Resources 的资源提供者实现
    /// </summary>
    public class ResourcesAssetProvider : IAssetProvider
    {
        private readonly AssetCatalog _catalog;
        private readonly ConcurrentDictionary<string, object> _handleCache;
        private readonly object _lockObject = new();

        public ResourcesAssetProvider(AssetCatalog catalog)
        {
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            _handleCache = new ConcurrentDictionary<string, object>();
        }

        public AssetHandle<T> Load<T>(AssetKey key, string scopeName = null) where T : Object
        {
            var cacheKey = GetCacheKey<T>(key);

            // 检查缓存
            if (_handleCache.TryGetValue(cacheKey, out var cachedHandle))
            {
                var handle = (AssetHandle<T>)cachedHandle;
                handle.AddReference();
                return handle;
            }

            lock (_lockObject)
            {
                // Double-check
                if (_handleCache.TryGetValue(cacheKey, out cachedHandle))
                {
                    var handle = (AssetHandle<T>)cachedHandle;
                    handle.AddReference();
                    return handle;
                }

                // 创建新句柄
                var newHandle = new AssetHandle<T>(key, scopeName);
                _handleCache[cacheKey] = newHandle;

                try
                {
                    // 获取资源路径
                    if (!_catalog.TryGetResourcesPath(key, out var resourcesPath))
                    {
                        newHandle.SetFailed($"Asset key not found in catalog: {key}");
                        return newHandle;
                    }

                    // 加载资源
                    var asset = Resources.Load<T>(resourcesPath);
                    if (asset == null)
                    {
                        newHandle.SetFailed($"Failed to load asset at path: {resourcesPath}");
                        return newHandle;
                    }

                    newHandle.SetCompleted(asset);
                    return newHandle;
                }
                catch (Exception ex)
                {
                    newHandle.SetFailed($"Exception during asset loading: {ex.Message}");
                    return newHandle;
                }
            }
        }

        public async Task<AssetHandle<T>> LoadAsync<T>(AssetKey key, string scopeName = null) where T : Object
        {
            var cacheKey = GetCacheKey<T>(key);

            // 检查缓存
            if (_handleCache.TryGetValue(cacheKey, out var cachedHandle))
            {
                var handle = (AssetHandle<T>)cachedHandle;
                handle.AddReference();

                // 等待加载完成
                while (handle.State == AssetLoadState.Loading) await Task.Yield();

                return handle;
            }

            AssetHandle<T> newHandle;
            lock (_lockObject)
            {
                // Double-check
                if (_handleCache.TryGetValue(cacheKey, out cachedHandle))
                {
                    var handle = (AssetHandle<T>)cachedHandle;
                    handle.AddReference();
                    newHandle = handle;
                }
                else
                {
                    // 创建新句柄
                    newHandle = new AssetHandle<T>(key, scopeName);
                    _handleCache[cacheKey] = newHandle;
                }
            }

            // 如果是新句柄，执行异步加载
            if (newHandle.State == AssetLoadState.Loading && newHandle.Asset == null)
                try
                {
                    // 获取资源路径
                    if (!_catalog.TryGetResourcesPath(key, out var resourcesPath))
                    {
                        newHandle.SetFailed($"Asset key not found in catalog: {key}");
                        return newHandle;
                    }

                    // 异步加载资源
                    var request = Resources.LoadAsync<T>(resourcesPath);
                    while (!request.isDone) await Task.Yield();

                    var asset = request.asset as T;
                    if (asset == null)
                    {
                        newHandle.SetFailed($"Failed to load asset at path: {resourcesPath}");
                        return newHandle;
                    }

                    newHandle.SetCompleted(asset);
                }
                catch (Exception ex)
                {
                    newHandle.SetFailed($"Exception during async asset loading: {ex.Message}");
                }
            else
                // 等待加载完成
                while (newHandle.State == AssetLoadState.Loading)
                    await Task.Yield();

            return newHandle;
        }

        public async Task<AssetHandle<T>[]> LoadBatchAsync<T>(AssetKey[] keys, string scopeName = null,
            IProgress<float> progress = null) where T : Object
        {
            if (keys == null || keys.Length == 0) return Array.Empty<AssetHandle<T>>();

            var handles = new AssetHandle<T>[keys.Length];
            var tasks = new Task<AssetHandle<T>>[keys.Length];

            // 启动所有加载任务
            for (var i = 0; i < keys.Length; i++) tasks[i] = LoadAsync<T>(keys[i], scopeName);

            // 等待完成并更新进度
            for (var i = 0; i < tasks.Length; i++)
            {
                handles[i] = await tasks[i];
                progress?.Report((float)(i + 1) / tasks.Length);
            }

            return handles;
        }

        public async Task<GameObject> InstantiateAsync(AssetKey key, Transform parent = null, string scopeName = null)
        {
            var handle = await LoadAsync<GameObject>(key, scopeName);

            if (!handle.IsValid)
                throw new InvalidOperationException(
                    $"Failed to load prefab for instantiation: {key}, Error: {handle.ErrorMessage}");

            return Object.Instantiate(handle.Asset, parent);
        }

        public void Release<T>(AssetHandle<T> handle) where T : Object
        {
            if (handle == null) return;

            var cacheKey = GetCacheKey<T>(handle.Key);
            var refCount = handle.RemoveReference();

            // 如果引用计数为0，从缓存中移除
            if (refCount <= 0)
            {
                _handleCache.TryRemove(cacheKey, out _);
                handle.Dispose();
            }
        }

        public AssetHandle<T> GetCachedHandle<T>(AssetKey key) where T : Object
        {
            var cacheKey = GetCacheKey<T>(key);
            return _handleCache.TryGetValue(cacheKey, out var handle) ? (AssetHandle<T>)handle : null;
        }

        public bool IsCached(AssetKey key)
        {
            var cacheKey = GetCacheKey<Object>(key);
            return _handleCache.ContainsKey(cacheKey);
        }

        private string GetCacheKey<T>(AssetKey key) where T : Object
        {
            return $"{key.Key}_{typeof(T).Name}";
        }
    }
}