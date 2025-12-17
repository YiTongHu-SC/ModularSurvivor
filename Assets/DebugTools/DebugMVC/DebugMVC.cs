using System;
using Core.Assets;
using Core.Events;
using GameLoop.Events;
using GameLoop.Game;
using UI.Framework;
using UnityEngine;

namespace DebugTools.DebugMVC
{
    public class DebugMvc : MonoBehaviour
    {
        public string DebugViewPrefabKey = "debug:ui:debug_view";
        public bool EnableDebugLogging = false;
        private bool _isInitialized = false;
        public DebugView DebugView { get; private set; }

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

        private async void CreateMvc(GameLoopEvents.BootComplete eventData)
        {
            try
            {
                if (_isInitialized) return;
                var debugScope = GameManager.Instance.AssetSystem.GetScope(AssetsScopeLabel.Debug);
                var handle = await debugScope.AcquireAsync<GameObject>(DebugViewPrefabKey);
                OnDebugViewLoaded(handle);
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception while creating Debug MVC: {e.Message}\n{e.StackTrace}");
            }
        }

        private void OnDebugViewLoaded(AssetHandle<GameObject> handle)
        {
            if (handle.IsValid)
            {
                // Successfully loaded, instantiate the debug view GameObject
                var debugViewInstance = Instantiate(handle.Asset, transform, false);
                // Optionally, you can parent it to this transform or do other setup
                DebugView = debugViewInstance.GetComponent<DebugView>();
                var controller = new DebugController();
                controller.Initialize(new DebugModel(), DebugView);
                MVCManager.Instance.RegisterController(controller);
                _isInitialized = true;

                if (EnableDebugLogging)
                {
                    Debug.Log($"DebugMvc: MVC initialized on {gameObject.name}");
                }
            }
            else
            {
                Debug.LogError(
                    $"Failed to load debug view prefab with key '{DebugViewPrefabKey}'. Error: {handle.ErrorMessage}");
            }
        }
    }
}