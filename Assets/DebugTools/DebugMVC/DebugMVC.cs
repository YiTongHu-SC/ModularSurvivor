using Core.Events;
using GameLoop.Events;
using UI.Framework;
using UnityEngine;

namespace DebugTools.DebugMVC
{
    public class DebugMvc : MonoBehaviour
    {
        // public string DebugViewPrefabKey = "debug:ui:debug_view";
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
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe<GameLoopEvents.BootComplete>(CreateMvc);
        }

        private void CreateMvc(GameLoopEvents.BootComplete eventData)
        {
            if (_isInitialized) return;
            if (EnableDebugLogging)
            {
                Debug.Log("DebugMvc: Creating Debug MVC");
            }

            MVCManager.Instance.Open<DebugController>();
        }
    }
}