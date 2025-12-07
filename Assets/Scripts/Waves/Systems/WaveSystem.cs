using System.Collections.Generic;
using Waves.Data;

namespace Waves.Systems
{
    public class WaveSystem
    {
        private readonly List<Wave> _waves = new();

        public void Initialize()
        {
        }

        /// <summary>
        /// 波次更新
        /// </summary>
        /// <param name="deltaTime"></param>
        public void UpdateWaves(float deltaTime)
        {
            foreach (var wave in _waves)
            {
                wave.UpdateWave(deltaTime);
            }
        }

        public void AddWave(WaveConfig waveConfig)
        {
            var wave = new Wave(waveConfig);
            _waves.Add(wave);
        }
    }
}