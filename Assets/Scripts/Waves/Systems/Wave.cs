using Waves.Data;
using Waves.Spawners;

namespace Waves.Systems
{
    public class Wave
    {
        private WaveConfig _waveConfig;
        private SimpleSpawner _spawner;

        public Wave(WaveConfig waveConfig)
        {
            _waveConfig = waveConfig;
            _spawner = new SimpleSpawner(waveConfig);
        }

        public void UpdateWave(float deltaTime)
        {
            _spawner.UpdateSpawner(deltaTime);
        }
    }
}