using System.Collections.Generic;
using Combat.Systems;
using StellarCore.Singleton;
using Waves.Data;

namespace Waves.Systems
{
    public class WaveManager : BaseInstance<WaveManager>
    {
        public WaveSystem WaveSystem { get; private set; } = new();
        private bool _initialized = false;

        public override void Initialize()
        {
            base.Initialize();
            WaveSystem.Initialize();
            _initialized = true;
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