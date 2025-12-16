using System;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Assets
{
    /// <summary>
    ///     资源提供者接口，统一加载门面
    /// </summary>
    public interface IAssetProvider
    {
        /// <summary>
        ///     同步加载资源（仅限小资源或编辑器工具使用）
        /// </summary>
        AssetHandle<T> Load<T>(AssetKey key, string scopeName = null) where T : Object;

        /// <summary>
        ///     异步加载单个资源
        /// </summary>
        Task<AssetHandle<T>> LoadAsync<T>(AssetKey key, string scopeName = null) where T : Object;

        /// <summary>
        ///     批量异步加载资源
        /// </summary>
        Task<AssetHandle<T>[]> LoadBatchAsync<T>(AssetKey[] keys, string scopeName = null,
            IProgress<float> progress = null) where T : Object;

        /// <summary>
        ///     实例化预制体（异步）
        /// </summary>
        Task<GameObject> InstantiateAsync(AssetKey key, Transform parent = null, string scopeName = null);

        /// <summary>
        ///     释放资源句柄引用
        /// </summary>
        void Release<T>(AssetHandle<T> handle) where T : Object;

        /// <summary>
        ///     获取缓存中的句柄（如果存在）
        /// </summary>
        AssetHandle<T> GetCachedHandle<T>(AssetKey key) where T : Object;

        /// <summary>
        ///     检查资源是否已缓存
        /// </summary>
        bool IsCached(AssetKey key);
    }
}