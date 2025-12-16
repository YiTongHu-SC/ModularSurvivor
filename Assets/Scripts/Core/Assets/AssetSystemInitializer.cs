using UnityEngine;

namespace Core.Assets
{
    /// <summary>
    /// 资源系统初始化器，在游戏启动时初始化资源系统
    /// </summary>
    public class AssetSystemInitializer : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private AssetCatalog _assetCatalog;
        [SerializeField] private AssetManifest _globalManifest;
        [SerializeField] private bool _loadGlobalAssetsOnStart = true;
        [SerializeField] private bool _createMemoryMaintenanceService = true;
        
        private AssetSystem _assetSystem;
        private bool _isInitialized;
        
        public AssetSystem AssetSystem => _assetSystem;
        public bool IsInitialized => _isInitialized;
        
        private void Awake()
        {
            InitializeAssetSystem();
            
            if (_createMemoryMaintenanceService)
            {
                CreateMemoryMaintenanceService();
            }
        }
        
        private async void Start()
        {
            if (_loadGlobalAssetsOnStart && _globalManifest != null)
            {
                await LoadGlobalAssets();
            }
        }
        
        private void InitializeAssetSystem()
        {
            if (_assetCatalog == null)
            {
                Debug.LogError("Asset catalog is not assigned!");
                return;
            }
            
            _assetSystem = new AssetSystem(_assetCatalog);
            _isInitialized = true;
            
            Debug.Log("[AssetSystem] Initialized successfully");
        }
        
        private void CreateMemoryMaintenanceService()
        {
            var go = new GameObject("MemoryMaintenanceService");
            DontDestroyOnLoad(go);
            
            go.AddComponent<MemoryMaintenanceService>();
            
            Debug.Log("[MemoryMaintenance] Service created");
        }
        
        private async System.Threading.Tasks.Task LoadGlobalAssets()
        {
            Debug.Log("[AssetSystem] Loading global assets...");
            
            var result = await _assetSystem.LoadManifestAsync(_globalManifest, AssetSystem.GlobalScopeName);
            
            if (result.HasFailures)
            {
                Debug.LogWarning($"[AssetSystem] Global assets loaded with {result.FailedCount} failures out of {result.TotalCount} assets");
            }
            else
            {
                Debug.Log($"[AssetSystem] Global assets loaded successfully ({result.SuccessCount} assets)");
            }
        }
        
        private void OnDestroy()
        {
            _assetSystem?.Dispose();
        }
        
        /// <summary>
        /// 获取内存统计信息（用于调试）
        /// </summary>
        public MemoryStats? GetMemoryStats()
        {
            return MemoryMaintenanceService.Instance?.GetMemoryStats();
        }
    }
}
