using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Core.AssetsTool;
using UnityEngine;

namespace Core.Assets
{
    /// <summary>
    ///     资源系统实用工具
    /// </summary>
    public static class AssetSystemUtils
    {
        /// <summary>
        ///     创建敌人族群清单
        /// </summary>
        public static AssetManifest CreateEnemyFamilyManifest(string familyName, string[] enemyKeys)
        {
            var manifest = ScriptableObject.CreateInstance<AssetManifest>();
            manifest.name = $"EnemyFamily_{familyName}";

            var entries = new List<ManifestEntry>();
            foreach (var enemyKey in enemyKeys)
                entries.Add(new ManifestEntry(enemyKey, AssetType.Prefab, 1f, false,
                    new[] { CommonTags.ENEMY, familyName }));

            // 使用反射设置私有字段（仅用于运行时创建）
            var entriesField =
                typeof(AssetManifest).GetField("_entries", BindingFlags.NonPublic | BindingFlags.Instance);
            entriesField?.SetValue(manifest, entries.ToArray());

            return manifest;
        }

        /// <summary>
        ///     检查作用域状态
        /// </summary>
        public static ScopeInfo GetScopeInfo(AssetScope scope)
        {
            if (scope == null)
                return new ScopeInfo { ScopeLabel = default, IsValid = false };

            return new ScopeInfo
            {
                ScopeLabel = scope.ScopeLabel,
                IsValid = !scope.IsDisposed,
                HandleCount = scope.HandleCount
            };
        }

        /// <summary>
        ///     获取所有作用域信息
        /// </summary>
        public static ScopeInfo[] GetAllScopeInfos()
        {
            var assetSystem = AssetSystem.Instance;
            if (assetSystem == null)
                return new ScopeInfo[0];

            var infos = new List<ScopeInfo>();

            // 获取已知的作用域
            var globalScope = assetSystem.GlobalScope;
            if (globalScope != null)
                infos.Add(GetScopeInfo(globalScope));

            var frontendScope = assetSystem.FrontendScope;
            if (frontendScope != null)
                infos.Add(GetScopeInfo(frontendScope));

            return infos.ToArray();
        }

        /// <summary>
        ///     预热常用资源类型
        /// </summary>
        public static async Task WarmupCommonAssets()
        {
            var assetSystem = AssetSystem.Instance;
            if (assetSystem?.GlobalScope == null)
                return;

            // 预加载一些常用的小资源
            var commonKeys = new[]
            {
                CommonKeys.UI_LOADING,
                CommonKeys.VFX_HIT_NORMAL
            };

            foreach (var key in commonKeys)
                try
                {
                    await assetSystem.GlobalScope.AcquireAsync<GameObject>(new AssetKey(key));
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to warmup asset '{key}': {ex.Message}");
                }
        }

        /// <summary>
        ///     构建关卡作用域名称
        /// </summary>
        public static string BuildLevelScopeName(string levelId, int runId)
        {
            return $"Level_{levelId}_{runId}";
        }

        /// <summary>
        ///     构建敌人键
        /// </summary>
        public static AssetKey BuildEnemyKey(string enemyType, AssetType assetType = AssetType.Prefab)
        {
            var suffix = assetType == AssetType.Prefab ? "prefab" : "config";
            return new AssetKey($"enemy:{enemyType}:{suffix}");
        }

        /// <summary>
        ///     构建UI键
        /// </summary>
        public static AssetKey BuildUIKey(string uiName)
        {
            return new AssetKey($"ui:{uiName}");
        }

        /// <summary>
        ///     构建音频键
        /// </summary>
        public static AssetKey BuildAudioKey(string audioType, string audioName)
        {
            return new AssetKey($"audio:{audioType}:{audioName}");
        }

        /// <summary>
        ///     常用资源键常量
        /// </summary>
        public static class CommonKeys
        {
            public const string UI_LOADING = "ui:loading";
            public const string UI_HUD = "ui:hud";
            public const string UI_MAIN_MENU = "ui:main_menu";

            public const string AUDIO_BGM_MENU = "audio:bgm:menu";
            public const string AUDIO_BGM_LEVEL = "audio:bgm:level";

            public const string VFX_HIT_NORMAL = "vfx:hit_normal";
            public const string VFX_EXPLOSION = "vfx:explosion";

            public const string BULLET_BASIC = "bullet:basic";
            public const string BULLET_PIERCING = "bullet:piercing";
        }

        /// <summary>
        ///     常用标签常量
        /// </summary>
        public static class CommonTags
        {
            public const string UI = "ui";
            public const string AUDIO = "audio";
            public const string VFX = "vfx";
            public const string ENEMY = "enemy";
            public const string WEAPON = "weapon";
            public const string LEVEL = "level";
            public const string COMMON = "common";
        }
    }

    /// <summary>
    ///     作用域信息结构
    /// </summary>
    public struct ScopeInfo
    {
        public AssetsScopeLabel ScopeLabel;
        public bool IsValid;
        public int HandleCount;

        public override string ToString()
        {
            return $"Scope '{ScopeLabel}': Valid={IsValid}, Handles={HandleCount}";
        }
    }
}