using System.Collections.Generic;
using Core.Events;
using UnityEngine;
using Waves.Data;

namespace Waves.Systems
{
    public class WaveConfigsHandler : MonoBehaviour
    {
        public List<WaveConfig> Waves = new();

        private void OnEnable()
        {
            EventManager.Instance.Subscribe<GameLoopEvents.CombatInitCompleteEvent>(InitCombat);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe<GameLoopEvents.CombatInitCompleteEvent>(InitCombat);
        }

        private void InitCombat(GameLoopEvents.CombatInitCompleteEvent obj)
        {
            StartWave();
        }

        public void StartWave()
        {
            Debug.Log("Starting Wave:Creating Waves");
            WaveManager.Instance.CreateWaves(Waves);
        }
    }
}