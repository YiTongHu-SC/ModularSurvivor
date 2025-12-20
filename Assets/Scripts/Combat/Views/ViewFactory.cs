using Core.Assets;
using Core.Units;
using Lean.Pool;
using UnityEngine;

namespace Combat.Views
{
    public class ViewFactory
    {
        public BaseUnitPresentation CreateUnitPresentation(int presentationId, ViewBaseData viewData)
        {
            // 加载PresentationData和Prefab
            var handler =
                AssetSystem.Instance.LevelScope.Acquire<PresentationConfig>("Level:EffectConfigs:PresentationConfig");

            if (!handler.Asset)
            {
                Debug.LogError($"PresentationData with ID {presentationId} not found.");
                return null;
            }

            var presentationConfig = handler.Asset;
            if (!presentationConfig.PresentationPrefab)
            {
                Debug.LogError($"UnitPresentation prefab with ID {presentationId} not found.");
                return null;
            }

            var instance = LeanPool.Spawn(presentationConfig.PresentationPrefab);
            var presentation = instance.GetComponent<BaseUnitPresentation>();
            if (presentation == null)
            {
                Debug.LogError($"BaseUnitPresentation component not found on prefab with ID {presentationId}.");
                Object.Destroy(instance);
                return null;
            }

            presentation.SetConfig(presentationConfig);
            presentation.SetViewData(viewData);
            return presentation;
        }
    }
}