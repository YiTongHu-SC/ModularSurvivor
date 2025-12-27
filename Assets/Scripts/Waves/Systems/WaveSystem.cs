using System.Collections.Generic;
using Core.GameInterface;
using Waves.Data;

namespace Waves.Systems
{
    public class WaveSystem : ISystem
    {
        private readonly List<Wave> _waves = new();

        /// <summary>
        /// 波次更新
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Tick(float deltaTime)
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

        public void Reset()
        {
            _waves.Clear();
        }
    }
}