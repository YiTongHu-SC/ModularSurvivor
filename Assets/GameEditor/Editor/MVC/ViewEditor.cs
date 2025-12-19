using UI.Framework;
using UI.Utils;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System;

namespace GameEditor.Editor.MVC
{
    [CustomEditor(typeof(BaseView<>), true)]
    public class ViewEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // 绘制默认的Inspector
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("BaseView Operations", EditorStyles.boldLabel);

            var baseView = target as MonoBehaviour;
            if (baseView == null) return;

            // 获取BaseView的类型信息
            var baseViewType = baseView.GetType();

            // 检查是否继承自BaseView<T>
            if (!IsBaseViewType(baseViewType)) return;

            EditorGUILayout.BeginHorizontal();
            // 刷新视图按钮
            if (GUILayout.Button("Clear Bind View"))
            {
                ClearViewProperties(baseView, baseViewType);
            }

            // 刷新视图按钮
            if (GUILayout.Button("Bind View Properties"))
            {
                BindViewProperties(baseView, baseViewType);
            }


            EditorGUILayout.EndHorizontal();

            // 显示视图状态信息
            ShowViewStatus(baseView, baseViewType);
        }

        /// <summary>
        /// 清除视图属性
        /// </summary>
        /// <param name="baseView"></param>
        /// <param name="baseViewType"></param>
        private void ClearViewProperties(MonoBehaviour baseView, Type baseViewType)
        {
            var clearCount = 0;

            // 获取所有公共字段
            var fields = baseViewType.GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fields)
            {
                // 检查字段是否继承自MonoBehaviour（UI组件通常都继承自MonoBehaviour）
                if (!typeof(MonoBehaviour).IsAssignableFrom(field.FieldType))
                    continue;

                // 如果字段有值，清除它
                if (field.GetValue(baseView) != null)
                {
                    field.SetValue(baseView, null);
                    clearCount++;
                    Debug.Log($"[ViewEditor] Cleared field: {field.Name}");
                }
            }

            Debug.Log($"[ViewEditor] Clear completed. Cleared fields: {clearCount}");

            // 标记对象为dirty以保存更改
            if (clearCount > 0)
            {
                EditorUtility.SetDirty(baseView);
            }
        }

        /// <summary>
        /// 自动绑定视图属性
        /// </summary>
        /// <param name="baseView"></param>
        /// <param name="baseViewType"></param>
        private void BindViewProperties(MonoBehaviour baseView, Type baseViewType)
        {
            var bindCount = 0;
            var failCount = 0;

            // 获取所有公共字段
            var fields = baseViewType.GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fields)
            {
                // 检查字段是否继承自MonoBehaviour（UI组件通常都继承自MonoBehaviour）
                if (!typeof(MonoBehaviour).IsAssignableFrom(field.FieldType))
                    continue;

                // 如果字段已经有值，跳过
                if (field.GetValue(baseView) != null)
                    continue;

                // 尝试绑定
                if (TryBindField(baseView, field))
                {
                    bindCount++;
                    Debug.Log($"[ViewEditor] Successfully bound field: {field.Name}");
                }
                else
                {
                    failCount++;
                    Debug.LogWarning($"[ViewEditor] Failed to bind field: {field.Name} (Type: {field.FieldType.Name})");
                }
            }

            Debug.Log($"[ViewEditor] Bind completed. Success: {bindCount}, Failed: {failCount}");

            // 标记对象为dirty以保存更改
            if (bindCount > 0)
            {
                EditorUtility.SetDirty(baseView);
            }
        }

        /// <summary>
        /// 尝试绑定单个字段
        /// </summary>
        /// <param name="baseView">目标视图</param>
        /// <param name="field">要绑定的字段</param>
        /// <returns>是否绑定成功</returns>
        private bool TryBindField(MonoBehaviour baseView, FieldInfo field)
        {
            try
            {
                // 获取UiTool的TryBind方法
                var uiToolType = typeof(UI.Utils.UiTool);
                var tryBindMethod = uiToolType.GetMethod("TryBind");

                if (tryBindMethod == null)
                {
                    Debug.LogError("[ViewEditor] UiTool.TryBind method not found");
                    return false;
                }

                // 创建泛型方法
                var genericMethod = tryBindMethod.MakeGenericMethod(field.FieldType);

                // 准备参数
                var parameters = new object[] { baseView.transform, field.Name, null };

                // 调用TryBind方法
                var result = (bool)genericMethod.Invoke(null, parameters);

                if (result)
                {
                    // 绑定成功，设置字段值
                    var boundComponent = parameters[2]; // out参数的值
                    field.SetValue(baseView, boundComponent);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ViewEditor] Error binding field {field.Name}: {ex.Message}");
                return false;
            }
        }

        private bool IsBaseViewType(Type type)
        {
            while (type != null)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition().Name.StartsWith("BaseView"))
                {
                    return true;
                }

                type = type.BaseType;
            }

            return false;
        }

        private void ShowViewStatus(MonoBehaviour baseView, Type baseViewType)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("View Status", EditorStyles.boldLabel);

            // 显示初始化状态
            var isInitializedProperty = baseViewType.GetProperty("IsInitialized");
            if (isInitializedProperty != null)
            {
                var isInitialized = (bool)isInitializedProperty.GetValue(baseView);
                EditorGUILayout.LabelField("Is Initialized:", isInitialized.ToString());
            }

            // 显示可见状态
            var isVisibleProperty = baseViewType.GetProperty("IsVisible");
            if (isVisibleProperty != null)
            {
                var isVisible = (bool)isVisibleProperty.GetValue(baseView);
                EditorGUILayout.LabelField("Is Visible:", isVisible.ToString());
            }

            // 显示GameObject状态
            EditorGUILayout.LabelField("GameObject Active:", baseView.gameObject.activeInHierarchy.ToString());
            EditorGUILayout.LabelField("Component Enabled:", baseView.enabled.ToString());

            // 显示AutoUpdate状态
            var autoUpdateField = baseViewType.GetField("AutoUpdate", BindingFlags.NonPublic | BindingFlags.Instance);
            if (autoUpdateField != null)
            {
                var autoUpdate = (bool)autoUpdateField.GetValue(baseView);
                EditorGUILayout.LabelField("Auto Update:", autoUpdate.ToString());
            }

            // 显示调试日志状态
            var debugField =
                baseViewType.GetField("EnableDebugLogging", BindingFlags.NonPublic | BindingFlags.Instance);
            if (debugField != null)
            {
                var debugEnabled = (bool)debugField.GetValue(baseView);
                EditorGUILayout.LabelField("Debug Logging:", debugEnabled.ToString());
            }
        }
    }
}