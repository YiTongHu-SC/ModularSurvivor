using System.Collections.Generic;
using Core.AssetsTool;
using GameLoop.Game;
using UnityEngine;

namespace GameLoop.Config
{
    [CreateAssetMenu(fileName = "GlobalConfig", menuName = "Game Config/GlobalConfig", order = -1)]
    public class GlobalConfig : ScriptableObject
    {
        public bool CreateMemoryMaintenanceService;
        public AssetCatalog AssetCatalog;
        public AssetManifest GlobalManifest;
        public AssetManifest DebugManifest;
        public AssetManifest LevelManifest;
        public List<LoadSceneMap> SceneMap = new();
        public string SystemSceneName = "Scenes/SystemScene";
        public string LoadingSceneName = "Scenes/LoadingScene";
        public float MinLoadingTime = 1.0f;
    }
}