using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Assets
{
    /// <summary>
    ///     资源系统，管理资源提供者和作用域
    /// </summary>
    public class AssetSystem : IDisposable
    {
        // 预定义的作用域名称
        public const string GlobalScopeName = "Global";
        public const string FrontendScopeName = "Frontend";
        private readonly object _lockObject = new();
        private readonly Dictionary<string, AssetScope> _scopes;
        private bool _disposed;

        public static AssetSystem Instance { get; private set; }

        /// <summary>
        ///     获取全局作用域
        /// </summary>
        public AssetScope GlobalScope => GetScope(GlobalScopeName);

        /// <summary>
        ///     获取前端作用域
        /// </summary>
        public AssetScope FrontendScope => GetScope(FrontendScopeName);

        /// <summary>
        ///     直接通过提供者访问（绕过作用域）
        /// </summary>
        public IAssetProvider Provider { get; }

        public AssetSystem(AssetCatalog catalog)
        {
            if (catalog == null)
                throw new ArgumentNullException(nameof(catalog));

            Provider = new ResourcesAssetProvider(catalog);
            _scopes = new Dictionary<string, AssetScope>();

            // 创建全局作用域
            CreateScope(GlobalScopeName);

            Instance = this;
        }

        public void Dispose()
        {
            if (_disposed) return;

            lock (_lockObject)
            {
                foreach (var scope in _scopes.Values) scope.Dispose();
                _scopes.Clear();
            }

            _disposed = true;

            if (Instance == this) Instance = null;
        }

        /// <summary>
        ///     创建作用域
        /// </summary>
        public AssetScope CreateScope(string name)
        {
            ThrowIfDisposed();

            lock (_lockObject)
            {
                if (_scopes.ContainsKey(name)) throw new InvalidOperationException($"Scope '{name}' already exists");

                var scope = new AssetScope(name, Provider);
                _scopes[name] = scope;
                return scope;
            }
        }

        /// <summary>
        ///     获取作用域
        /// </summary>
        public AssetScope GetScope(string name)
        {
            ThrowIfDisposed();

            lock (_lockObject)
            {
                return _scopes.GetValueOrDefault(name);
            }
        }

        /// <summary>
        ///     获取或创建作用域
        /// </summary>
        public AssetScope GetOrCreateScope(string name)
        {
            var scope = GetScope(name);
            return scope ?? CreateScope(name);
        }

        /// <summary>
        ///     释放作用域
        /// </summary>
        public bool ReleaseScope(string name)
        {
            ThrowIfDisposed();

            lock (_lockObject)
            {
                if (_scopes.TryGetValue(name, out var scope))
                {
                    scope.Dispose();
                    _scopes.Remove(name);
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        ///     创建关卡作用域（带唯一ID）
        /// </summary>
        public AssetScope CreateLevelScope(string levelId, int runId)
        {
            var scopeName = $"Level_{levelId}_{runId}";
            return CreateScope(scopeName);
        }

        /// <summary>
        ///     确保前端作用域存在
        /// </summary>
        public AssetScope EnsureFrontendScope()
        {
            return GetScope(FrontendScopeName) ?? CreateScope(FrontendScopeName);
        }

        /// <summary>
        ///     加载清单中的所有资源到指定作用域
        /// </summary>
        public async Task<LoadManifestResult> LoadManifestAsync(AssetManifest manifest, string scopeName,
            IProgress<float> progress = null)
        {
            ThrowIfDisposed();

            if (manifest == null)
                throw new ArgumentNullException(nameof(manifest));

            var scope = GetOrCreateScope(scopeName);
            var result = new LoadManifestResult();
            var entries = manifest.Entries;

            for (var i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                var key = new AssetKey(entry.Key);

                try
                {
                    var handle = await scope.AcquireAsync<Object>(key);
                    if (handle.IsValid)
                    {
                        result.SuccessCount++;
                        result.LoadedKeys.Add(key);
                    }
                    else
                    {
                        result.FailedCount++;
                        result.FailedKeys.Add(key);

                        HandleLoadFailure(manifest.FailureStrategy, key, handle.ErrorMessage, entry.IsRequired);
                    }
                }
                catch (Exception ex)
                {
                    result.FailedCount++;
                    result.FailedKeys.Add(key);

                    HandleLoadFailure(manifest.FailureStrategy, key, ex.Message, entry.IsRequired);
                }

                progress?.Report((float)(i + 1) / entries.Length);
            }

            return result;
        }

        private void HandleLoadFailure(LoadFailureStrategy strategy, AssetKey key, string error, bool isRequired)
        {
            var message = $"Failed to load asset '{key}': {error}";

            switch (strategy)
            {
                case LoadFailureStrategy.Ignore:
                    break;
                case LoadFailureStrategy.LogWarning:
                    Debug.LogWarning(message);
                    break;
                case LoadFailureStrategy.LogError:
                    Debug.LogError(message);
                    break;
                case LoadFailureStrategy.ThrowException:
                    if (isRequired)
                        throw new InvalidOperationException(message);
                    Debug.LogWarning(message);
                    break;
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(AssetSystem));
        }
    }

    /// <summary>
    ///     清单加载结果
    /// </summary>
    public class LoadManifestResult
    {
        public int SuccessCount { get; internal set; }
        public int FailedCount { get; internal set; }
        public List<AssetKey> LoadedKeys { get; } = new();
        public List<AssetKey> FailedKeys { get; } = new();

        public bool HasFailures => FailedCount > 0;
        public int TotalCount => SuccessCount + FailedCount;
        public float SuccessRate => TotalCount > 0 ? (float)SuccessCount / TotalCount : 0f;
    }
}