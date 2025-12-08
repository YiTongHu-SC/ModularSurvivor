using System;
using Combat.Systems;
using Core.Units;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace DebugTools
{
    public class DebugPanel : MonoBehaviour
    {
        private Transform PanelTransform;
        private Button DamageToHeroButton;
        private InputSystem_Actions _inputActions;

        private void OnEnable()
        {
            if (_inputActions == null)
                _inputActions = new InputSystem_Actions();
            _inputActions.UI.Enable();
        }

        private void OnDisable()
        {
            _inputActions.UI.Disable();
        }

        private void Start()
        {
            _inputActions.UI.DebugTool.performed += ToggleDebugToolUI;
            RegisterUIElements();
            PanelTransform.gameObject.SetActive(false);
        }

        private void ToggleDebugToolUI(InputAction.CallbackContext obj)
        {
            PanelTransform.gameObject.SetActive(!PanelTransform.gameObject.activeSelf);
        }

        private void RegisterUIElements()
        {
            foreach (var child in GetComponentsInChildren<Transform>(true))
            {
                TryBindComponent(child, "PanelTransform", ref PanelTransform);
                TryBindComponent(child, "DamageToHeroButton", ref DamageToHeroButton, ApplyDamageToHero);
            }
        }

        private bool TryBindComponent<T>(Transform trans, string elementName, ref T component,
            UnityAction action = null)
            where T : Component
        {
            if (trans.name == elementName)
            {
                component = trans.GetComponent<T>();
                if (component != null && component is Button button && action != null)
                {
                    button.onClick.AddListener(action);
                }

                return true;
            }

            return false;
        }

        private void ApplyDamageToHero()
        {
            const float damageAmount = 10;
            // 假设有一个方法可以获取主角的UnitData
            var heroUnitData = UnitManager.Instance.HeroUnitData;
            if (heroUnitData != null)
            {
                CombatManager.Instance.DamageSystem.TryApplyDamage(heroUnitData, damageAmount); // 对主角造成10点伤害
                Debug.Log($"Applied 10 damage to hero. New Health: {heroUnitData.Health}");
            }
        }
    }
}