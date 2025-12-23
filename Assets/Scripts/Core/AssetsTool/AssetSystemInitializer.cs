using System.Threading.Tasks;
using UnityEngine;

namespace Core.AssetsTool
{
    /// <summary>
    /// 资源系统初始化器，在游戏启动时初始化资源系统
    /// </summary>
    public class AssetSystemInitializer : MonoBehaviour
    {
        [Header("Configuration")] [SerializeField]
        private AssetCatalog _assetCatalog;

        [SerializeField] private AssetManifest _globalManifest;
        [SerializeField] private bool _loadGlobalAssetsOnStart = true;
        [SerializeField] private bool _createMemoryMaintenanceService = true;

        public AssetSystem AssetSystem { get; private set; }

        public bool IsInitialized { get; private set; }

        private void Awake()
        {
            InitializeAssetSystem();

            if (_createMemoryMaintenanceService) CreateMemoryMaintenanceService();
        }

        private async void Start()
        {
            if (_loadGlobalAssetsOnStart && _globalManifest != null) await LoadGlobalAssets();
        }

        private void OnDestroy()
        {
            AssetSystem?.Dispose();
        }

        private void InitializeAssetSystem()
        {
            if (_assetCatalog == null)
            {
                Debug.LogError("Asset catalog is not assigned!");
                return;
            }

            AssetSystem = new AssetSystem(_assetCatalog);
            IsInitialized = true;

            Debug.Log("[AssetSystem] Initialized successfully");
        }

        private void CreateMemoryMaintenanceService()
        {
            var go = new GameObject("MemoryMaintenanceService");
            DontDestroyOnLoad(go);

            go.AddComponent<MemoryMaintenanceService>();

            Debug.Log("[MemoryMaintenance] Service created");
        }

        private async Task LoadGlobalAssets()
        {
            Debug.Log("[AssetSystem] Loading global assets...");

            var result = await AssetSystem.LoadManifestAsync(_globalManifest, AssetsScopeLabel.Global);

            if (result.HasFailures)
                Debug.LogWarning(
                    $"[AssetSystem] Global assets loaded with {result.FailedCount} failures out of {result.TotalCount} assets");
            else
                Debug.Log($"[AssetSystem] Global assets loaded successfully ({result.SuccessCount} assets)");
        }

        /// <summary>
        ///     获取内存统计信息（用于调试）
        /// </summary>
        public MemoryStats? GetMemoryStats()
        {
            return MemoryMaintenanceService.Instance?.GetMemoryStats();
        }
    }
}