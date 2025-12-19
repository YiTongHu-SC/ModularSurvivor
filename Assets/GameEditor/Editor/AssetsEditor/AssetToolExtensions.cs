using System;
using Core.Assets;
using UnityEngine;
using Object = System.Object;

namespace GameEditor.Editor.AssetsEditor
{
    public static class AssetToolExtensions
    {
        /// <summary>
        /// 资源路径转换为键值,命名规则
        /// 键值：prefix:asset_name (不含扩展名)
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static string GetAssetPathKey(string assetPath)
        {
            // 提取 Resources 目录下的相对路径
            var resPath = GetAssetPathResource(assetPath);
            if (string.IsNullOrEmpty(resPath))
            {
                Debug.Log($"Asset not in Resources folder: {assetPath}");
                return null;
            }

            // 1.去掉扩展名
            var extensionIndex = resPath.LastIndexOf('.');
            if (extensionIndex >= 0)
            {
                resPath = resPath.Substring(0, extensionIndex);
            }

            // 2.替换路径分隔符为冒号
            resPath = resPath.Replace('/', ':').Replace('\\', ':');

            // 3.大写改为下划线 camelCase to snake_case
            // var result = new System.Text.StringBuilder();
            // for (int i = 0; i < resPath.Length; i++)
            // {
            //     char currentChar = resPath[i];
            //     if (char.IsUpper(currentChar) && i > 0 &&
            //         resPath[i - 1] != ':' && resPath[i - 1] != '_' && !char.IsUpper(resPath[i - 1]))
            //     {
            //         result.Append('_');
            //     }
            //
            //     result.Append(currentChar);
            // }

            // 4.转为全小写
            // return result.ToString().ToLowerInvariant();
            return resPath;
        }

        public static string GetAssetPathResource(string assetPath)
        {
            var resourcesIndex = assetPath.IndexOf("Resources/", StringComparison.Ordinal);
            if (resourcesIndex >= 0)
            {
                // 提取 Resources/ 后面的路径，并去掉扩展名
                var relativePath = assetPath.Substring(resourcesIndex + "Resources/".Length);
                var extensionIndex = relativePath.LastIndexOf('.');
                if (extensionIndex >= 0)
                {
                    relativePath = relativePath.Substring(0, extensionIndex);
                }

                return relativePath;
            }

            Debug.LogWarning($"Asset not in Resources folder: {assetPath}");
            return null; // 资产不在 Resources 目录下
        }

        public static AssetType GetAssetType(Object asset)
        {
            if (asset is GameObject)
                return AssetType.Prefab;
            if (asset is ScriptableObject)
                return AssetType.ScriptableObject;
            if (asset is Sprite)
                return AssetType.Sprite;
            if (asset is AudioClip)
                return AssetType.AudioClip;
            if (asset is TextAsset)
                return AssetType.TextAsset;
            if (asset is Material)
                return AssetType.Material;
            if (asset is Shader)
                return AssetType.Shader;
            if (asset is Texture)
                return AssetType.Texture;
            return AssetType.Other;
        }
    }
}