using Combat.Ability;
using Combat.Buff;
using Combat.Data;
using Combat.Systems;
using Core.Units;
using UnityEngine;
using Waves.Data;

namespace Waves.Spawners
{
    public class SimpleSpawner : BaseSpawner
    {
        private float _spawnRadius;
        private float _timer;
        const float DeathDelayTime = 10f;

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
            unitData.SetHealth(10);
            unitData.MoveSpeed = 1;
            unitData.MovementStrategy = "StraightChase";
            unitData.Group = GroupType.Enemy;
            // set unit collision data
            unitData.CollisionData = new UnitCollisionData
            {
                AreaType = CollisionAreaType.Circle,
                Radius = 1
            };

            // Spawn unit
            Spawn(actorData, unitData);
            // apply ability
            var abilityData = new HitOnceOnCollisionData
            {
                DamageAmount = 10,
                HitCooldown = 0.2f
            };
            CombatManager.Instance.AbilitySystem.ApplyAbility(AbilityType.HitOnceOnCollision, abilityData,
                unitData.GUID);
            // apply Buff
            var buffData = new BuffData(0, "DelayDeath", BuffType.DelayDeath, DeathDelayTime);
            CombatManager.Instance.BuffSystem.ApplyBuff(BuffType.DelayDeath, buffData, unitData.GUID);
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