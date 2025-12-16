using UnityEngine;

namespace GameEditor.RunTime.Config
{
    [CreateAssetMenu(fileName = "AssetsToolEditorConfig", menuName = "EditorConfig/AssetsToolEditorConfig", order = 0)]
    public class AssetsToolEditorConfig : ScriptableObject
    {
        public string AssetCatalogFile = "Assets/Configs/AssetsConfig/AssetCatalogGlobal.asset";
        public string AssetTargetPath = "Assets/Resources";
    }
}