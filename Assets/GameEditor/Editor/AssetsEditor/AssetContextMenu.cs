using UnityEditor;
using UnityEngine;

namespace GameEditor.Editor.AssetsEditor
{
    public static class AssetContextMenu
    {
        [MenuItem("Assets/Copy Resource PathKey", false, 200)]
        private static void CopyResourcePathKey()
        {
            Object[] selected = Selection.objects;
            if (selected.Length == 0) return;
            var target = selected[0];
            var path = AssetDatabase.GetAssetPath(target);
            var key = AssetToolExtensions.GetAssetPathKey(path);
            // 输出结果到控制台
            Debug.Log($"Resource PathKey for asset '{target.name}': {key}");
            // 复制到剪贴板
            EditorGUIUtility.systemCopyBuffer = key;
            Debug.Log("Resource PathKey copied to clipboard.");
        }

        /// <summary>
        /// 校验/控制该菜单项是否可点击,只有选中单个资源时才可用
        /// </summary>
        /// <returns></returns>
        [MenuItem("Assets/Copy Resource PathKey", true)]
        private static bool CopyResourcePathKey_Validate()
        {
            return Selection.objects != null && Selection.objects.Length == 1;
        }
    }
}