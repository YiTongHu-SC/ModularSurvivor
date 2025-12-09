using Lean.Pool;
using UnityEngine;

namespace Combat.Views
{
    public class ViewFactory
    {
        public BaseUnitPresentation CreateUnitPresentation(int presentationId)
        {
            // 加载PresentationData和Prefab
            // TODO: 优化资源加载方式，避免直接使用Resources.Load
            var data = Resources.Load<PresentationData>($"PresentationData/Presentation_{presentationId}");
            if (data == null)
            {
                Debug.LogError($"PresentationData with ID {presentationId} not found.");
                return null;
            }

            var prefab = Resources.Load<GameObject>($"Prefabs/UnitPresentations/UnitPresentation_{presentationId}");
            if (prefab == null)
            {
                Debug.LogError($"UnitPresentation prefab with ID {presentationId} not found.");
                return null;
            }

            var instance = LeanPool.Spawn(prefab);
            var presentation = instance.GetComponent<BaseUnitPresentation>();
            if (presentation == null)
            {
                Debug.LogError($"BaseUnitPresentation component not found on prefab with ID {presentationId}.");
                GameObject.Destroy(instance);
                return null;
            }

            presentation.SetData(data);
            return presentation;
        }
    }
}