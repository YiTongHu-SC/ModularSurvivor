using System;
using System.Collections.Generic;
using Core.Events;
using UnityEngine;
using Waves.Data;

namespace Waves.Systems
{
    public class WaveConfigsHandler : MonoBehaviour, IEventListener<GameEvents.GameInitializedEvent>
    {
        public List<WaveConfig> Waves = new();

        private void OnEnable()
        {
            if (EventManager.Instance)
            {
                EventManager.Instance.Subscribe<GameEvents.GameInitializedEvent>(this);
            }
        }

        private void OnDisable()
        {
            if (EventManager.Instance)
            {
                EventManager.Instance.Unsubscribe<GameEvents.GameInitializedEvent>(this);
            }
        }

        public void StartWave()
        {
            Debug.Log("Starting Wave:Creating Waves");
            WaveManager.Instance.CreateWaves(this);
        }

        public void OnEventReceived(GameEvents.GameInitializedEvent eventData)
        {
            Debug.Log("OnEventReceived");
            StartWave();
        }
    }
}