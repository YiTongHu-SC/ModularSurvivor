using Core.Events;
using Core.Input;
using UI.Framework;
using UnityEngine;

namespace DebugTools.DebugMVC
{
    public class DebugMvc : MonoBehaviour
    {
        public bool InitOpen = false;
        public bool EnableDebugLogging = false;
        private bool _isInitialized = false;

        private void Awake()
        {
            _isInitialized = false;
        }

        private void OnEnable()
        {
            if (EventManager.Instance == null) return;
            EventManager.Instance.Subscribe<GameLoopEvents.BootComplete>(CreateMvc);
            EventManager.Instance.Subscribe<InputEvents.InputDebugUIEvent>(ProcessDebugInput);
        }

        private void ProcessDebugInput(InputEvents.InputDebugUIEvent obj)
        {
            if (obj.Context == InputEvents.DebugContext.ToggleDebugUI)
            {
                var controller = MvcManager.Instance.GetController<DebugController>();
                if (controller != null)
                {
                    var isOpen = controller.IsOpen;
                    if (isOpen)
                    {
                        MvcManager.Instance.Close<DebugController>();
                    }
                    else
                    {
                        MvcManager.Instance.Open<DebugController>();
                    }
                }
                else
                {
                    MvcManager.Instance.Open<DebugController>();
                }
            }
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe<GameLoopEvents.BootComplete>(CreateMvc);
            EventManager.Instance.Unsubscribe<InputEvents.InputDebugUIEvent>(ProcessDebugInput);
        }

        private void CreateMvc(GameLoopEvents.BootComplete eventData)
        {
            if (_isInitialized) return;
            if (EnableDebugLogging)
            {
                Debug.Log("DebugMvc: Creating Debug MVC");
            }

            if (InitOpen)
            {
                MvcManager.Instance.Open<DebugController>();
            }
        }
    }
}