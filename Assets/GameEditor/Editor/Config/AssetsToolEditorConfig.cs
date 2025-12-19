using Core.AssetsTool;
using UnityEngine;

namespace GameEditor.Editor.Config
{
    [CreateAssetMenu(fileName = "AssetsToolEditorConfig", menuName = "EditorConfig/AssetsToolEditorConfig", order = 0)]
    public class AssetsToolEditorConfig : ScriptableObject
    {
        public AssetCatalog AssetCatalogFile;
        public string AssetTargetPath = "Assets/Resources";
    }
}