using Core.Assets;
using UnityEngine;

namespace GameLoop.Config
{
    [CreateAssetMenu(fileName = "GlobalConfig", menuName = "Game Config/GlobalConfig", order = -1)]
    public class GlobalConfig : ScriptableObject
    {
        public bool LoadGlobalAssetsOnStart;
        public bool CreateMemoryMaintenanceService;
        public AssetsScopeLabel GlobalScopeLabel = default;
        public AssetManifest GlobalManifest;
        public AssetCatalog AssetCatalog;
    }
}