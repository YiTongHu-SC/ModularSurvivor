using System.Collections.Generic;
using Core.Assets;
using Core.AssetsTool;
using GameLoop.Game;
using UnityEngine;

namespace GameLoop.Config
{
    [CreateAssetMenu(fileName = "GlobalConfig", menuName = "Game Config/GlobalConfig", order = -1)]
    public class GlobalConfig : ScriptableObject
    {
        public bool CreateMemoryMaintenanceService;
        public AssetsScopeLabel GlobalScopeLabel = default;
        public AssetCatalog AssetCatalog;
        public AssetManifest GlobalManifest;
        public AssetManifest DebugManifest;
        public List<LoadSceneMap> SceneMap = new();
        public string StaticSceneName = "Scenes/SystemScene";
        public string LoadingSceneName = "Scenes/LoadingScene";
    }
}