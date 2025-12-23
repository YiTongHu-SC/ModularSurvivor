using Core.Assets;
using Core.Units;
using Lean.Pool;
using UnityEngine;

namespace Combat.Views
{
    public class ViewFactory
    {
        public BaseUnitPresentation CreateUnitPresentation(string presentationKey, ViewBaseData eventDataViewData)
        {
            // 加载PresentationData和Prefab
            var handler =
                AssetSystem.Instance.LevelScope.Acquire<PresentationConfig>("Level:EffectConfigs:PresentationConfig");

            if (!handler.Asset)
            {
                Debug.LogError($"PresentationData with ID {presentationKey} not found.");
                return null;
            }

            var presentationConfig = handler.Asset;
            if (!presentationConfig.PresentationPrefab)
            {
                Debug.LogError($"UnitPresentation prefab with ID {presentationKey} not found.");
                return null;
            }

            var instance = LeanPool.Spawn(presentationConfig.PresentationPrefab);
            var presentation = instance.GetComponent<BaseUnitPresentation>();
            if (presentation == null)
            {
                Debug.LogError($"BaseUnitPresentation component not found on prefab with ID {presentationKey}.");
                Object.Destroy(instance);
                return null;
            }

            presentation.SetConfig(presentationConfig);
            presentation.SetViewData(eventDataViewData);
            return presentation;
        }
    }
}