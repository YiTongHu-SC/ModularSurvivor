using System;
using System.Collections.Generic;
using Core.Events;
using UnityEngine;
using Waves.Data;

namespace Waves.Systems
{
    public class WaveConfigsHandler : MonoBehaviour
    {
        public List<WaveConfig> Waves = new();

        public void StartWave()
        {
            Debug.Log("Starting Wave:Creating Waves");
            WaveManager.Instance.CreateWaves(this);
        }
    }
}