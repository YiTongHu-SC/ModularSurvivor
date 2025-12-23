using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Assets;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.AssetsTool
{
    /// <summary>
    ///     资源系统，管理资源提供者和作用域
    /// </summary>
    public class AssetSystem : IDisposable
    {
        // 预定义的作用域名称
        private readonly object _lockObject = new();
        private readonly Dictionary<AssetsScopeLabel, AssetScope> _scopes;
        private bool _disposed;

        public static AssetSystem Instance { get; private set; }

        /// <summary>
        ///     获取全局作用域
        /// </summary>
        public AssetScope GlobalScope => GetScope(AssetsScopeLabel.Global);

        /// <summary>
        ///     获取前端作用域
        /// </summary>
        public AssetScope FrontendScope => GetScope(AssetsScopeLabel.Frontend);

        public AssetScope LevelScope => GetScope(AssetsScopeLabel.Level);

        /// <summary>
        ///     直接通过提供者访问（绕过作用域）
        /// </summary>
        public IAssetProvider Provider { get; }

        public AssetSystem(AssetCatalog catalog)
        {
            if (catalog == null)
                throw new ArgumentNullException(nameof(catalog));

            Provider = new ResourcesAssetProvider(catalog);
            _scopes = new Dictionary<AssetsScopeLabel, AssetScope>();

            // 创建全局作用域
            CreateScope(AssetsScopeLabel.Global);

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
        public AssetScope CreateScope(AssetsScopeLabel label)
        {
            ThrowIfDisposed();

            lock (_lockObject)
            {
                if (_scopes.ContainsKey(label)) throw new InvalidOperationException($"Scope '{label}' already exists");

                var scope = new AssetScope(label, Provider);
                _scopes[label] = scope;
                return scope;
            }
        }

        /// <summary>
        ///     获取作用域
        /// </summary>
        public AssetScope GetScope(AssetsScopeLabel label)
        {
            ThrowIfDisposed();

            lock (_lockObject)
            {
                return _scopes.GetValueOrDefault(label);
            }
        }

        /// <summary>
        ///     获取或创建作用域
        /// </summary>
        public AssetScope GetOrCreateScope(AssetsScopeLabel label)
        {
            var scope = GetScope(label);
            return scope ?? CreateScope(label);
        }

        /// <summary>
        ///     释放作用域
        /// </summary>
        public bool ReleaseScope(AssetsScopeLabel label)
        {
            ThrowIfDisposed();

            lock (_lockObject)
            {
                if (_scopes.TryGetValue(label, out var scope))
                {
                    scope.Dispose();
                    _scopes.Remove(label);
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
            // TODO: 使用组合标签来区分不同关卡实例
            return CreateScope(AssetsScopeLabel.Level);
        }

        /// <summary>
        ///     确保前端作用域存在
        /// </summary>
        public AssetScope EnsureFrontendScope()
        {
            return GetScope(AssetsScopeLabel.Frontend) ?? CreateScope(AssetsScopeLabel.Frontend);
        }

        /// <summary>
        ///     加载清单中的所有资源到指定作用域
        /// </summary>
        public async Task<LoadManifestResult> LoadManifestAsync(AssetManifest manifest, AssetsScopeLabel scopeLabel,
            IProgress<float> progress = null)
        {
            ThrowIfDisposed();

            if (manifest == null)
                throw new ArgumentNullException(nameof(manifest));

            var scope = GetOrCreateScope(scopeLabel);
            var result = new LoadManifestResult();
            var entries = manifest.Entries;

            for (var i = 0; i < entries.Count; i++)
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

                progress?.Report((float)(i + 1) / entries.Count);
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

    [Serializable]
    public enum AssetsScopeLabel
    {
        Global = 0,
        Frontend = 1,
        Level = 2,
        Debug = 3
    }
}