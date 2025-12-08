using Combat.Data;
using Core.Units;
using UnityEngine;
using Waves.Data;

namespace Waves.Spawners
{
    public class SimpleSpawner : BaseSpawner
    {
        private float _spawnRadius;
        private float _timer;

        public SimpleSpawner(WaveConfig config) : base(config)
        {
            _spawnRadius = config.SpawnRadius;
            _timer = 0;
        }

        public override void UpdateSpawner(float deltaTime)
        {
            _timer += deltaTime;
            if (_timer >= Config.SpawnInterval)
            {
                SpawnEnemy();
                _timer = 0f;
            }
        }

        private void SpawnEnemy()
        {
            var angle = Random.Range(0f, 360f);
            var radian = angle * Mathf.Deg2Rad;
            var spawnPosition = new Vector2(
                Mathf.Cos(radian) * _spawnRadius,
                Mathf.Sin(radian) * _spawnRadius);
            var actorData = GetActorDataById(Config.EnemyID);
            var unitData = new UnitData(spawnPosition, 0); // 根据需要初始化UnitData
            unitData.MoveSpeed = 1;
            unitData.MovementStrategy = "StraightChase";
            Spawn(actorData, unitData);
            Debug.Log($"Spawning enemy {actorData.ActorId} from SimpleSpawner");
        }

        private ActorData GetActorDataById(int enemyID)
        {
            // TODO: 暂时通过Resources加载，后续可以考虑资源管理器
            var actorData = Resources.Load<ActorData>($"ActorConfigs/Actor_{enemyID}");
            return actorData;
        }
    }
}