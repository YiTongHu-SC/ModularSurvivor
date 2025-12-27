using System;
using System.Collections.Generic;
using Combat.Systems;
using Core.GameInterface;
using StellarCore.Singleton;
using Waves.Data;

namespace Waves.Systems
{
    public class WaveManager : BaseInstance<WaveManager>, IManager
    {
        public WaveSystem WaveSystem { get; private set; }

        public bool IsInitialized { get; private set; }

        public override void Initialize()
        {
            base.Initialize();
            WaveSystem = new WaveSystem();
            IsInitialized = true;
        }

        public void Reset()
        {
            IsInitialized = false;
            WaveSystem = null;
        }

        public void CreateWaves(List<WaveConfig> waves)
        {
            foreach (var waveConfig in waves)
            {
                WaveSystem.AddWave(waveConfig);
            }
        }

        public void Tick(float deltaTime)
        {
            if (CombatManager.Instance.CurrentState == CombatState.InCombat)
            {
                WaveSystem.UpdateWaves(deltaTime);
            }
        }
    }
}