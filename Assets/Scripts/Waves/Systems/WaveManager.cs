using StellarCore.Singleton;

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

        public void CreateWaves(WaveConfigsHandler waveConfigsHandler)
        {
            foreach (var waveConfig in waveConfigsHandler.Waves)
            {
                WaveSystem.AddWave(waveConfig);
            }
        }

        public void Tick(float deltaTime)
        {
            WaveSystem.UpdateWaves(deltaTime);
        }
    }
}