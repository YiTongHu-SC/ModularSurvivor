using Combat.Ability.Data;
using Combat.Buff;
using Combat.Data;
using Combat.Effect;
using Combat.Systems;
using Core.Assets;
using Core.AssetsTool;
using Core.Units;
using UnityEngine;
using Waves.Data;

namespace Waves.Spawners
{
    public class SimpleSpawner : BaseSpawner
    {
        private float _spawnRadius;
        private float _timer;
        const float DeathDelayTime = 100f;
        private int _count;
        private bool _isWaveEnd;

        public SimpleSpawner(WaveConfig config) : base(config)
        {
            _spawnRadius = config.SpawnRadius;
            _timer = 0;
            _count = 0;
            _isWaveEnd = false;
        }

        public override void UpdateSpawner(float deltaTime)
        {
            if (_isWaveEnd) return;
            _timer += deltaTime;
            if (_timer >= Config.SpawnInterval)
            {
                for (var i = 0; i < Config.EnemyCountEachSubWave; i++)
                {
                    if (_count >= Config.TotalEnemies)
                    {
                        _isWaveEnd = true;
                        break;
                    }
                    SpawnEnemy();
                }

                _timer = 0f;
            }
        }

        private void SpawnEnemy()
        {
            _count += 1;
            // TODO: 后面应该通过配置表直接读取敌人数据
            var angle = Random.Range(0f, 360f);
            var radian = angle * Mathf.Deg2Rad;
            var spawnPosition = new Vector2(
                Mathf.Cos(radian) * _spawnRadius,
                Mathf.Sin(radian) * _spawnRadius);

            var enemy = CombatManager.Instance.ActorFactory.SpawnCharacter("actor_enemy_100", spawnPosition);
            // apply Buff
            Debug.Log($"Spawning enemy {enemy.name} from SimpleSpawner");
        }
    }
}