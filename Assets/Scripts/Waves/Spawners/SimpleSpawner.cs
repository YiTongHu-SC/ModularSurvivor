using Combat.Ability;
using Combat.Ability.Data;
using Combat.Buff;
using Combat.Data;
using Combat.Systems;
using Core.Assets;
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
                    SpawnEnemy();
                    _count += 1;
                    if (_count >= Config.TotalEnemies)
                    {
                        _isWaveEnd = true;
                        break;
                    }
                }

                _timer = 0f;
            }
        }

        private void SpawnEnemy()
        {
            // TODO: 后面应该通过配置表直接读取敌人数据
            var angle = Random.Range(0f, 360f);
            var radian = angle * Mathf.Deg2Rad;
            var spawnPosition = new Vector2(
                Mathf.Cos(radian) * _spawnRadius,
                Mathf.Sin(radian) * _spawnRadius);
            var actorData = GetActorDataById(Config.EnemyID);
            var unitData = new UnitData(spawnPosition, 0)
            {
                Group = GroupType.Enemy,

                ModelView = new UnitModelView()
                {
                    Height = 1,
                    CenterOffset = 0.5f,
                    Radius = 0.5f
                },
                CollisionData = new UnitCollisionData()
                {
                    AreaType = CollisionAreaType.Circle,
                    Radius = 0.5f
                },
                MoveSpeed = 1,
                MovementStrategy = "StraightChase",
            };

            unitData.SetHealth(10);
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
            // chase hero
            var chaseAbilityData = new AbilityData
            {
                ID = 3,
                AbilityType = AbilityType.ChaseHero
            };
            CombatManager.Instance.AbilitySystem.ApplyAbility(AbilityType.ChaseHero, chaseAbilityData, unitData.GUID);
            // apply Buff
            var buffData = new BuffData(0, "DelayDeath", BuffType.DelayDeath, DeathDelayTime);
            CombatManager.Instance.BuffSystem.ApplyBuff(BuffType.DelayDeath, buffData, unitData.GUID);
            Debug.Log($"Spawning enemy {actorData.ActorId} from SimpleSpawner");
        }

        private ActorData GetActorDataById(int enemyID)
        {
            // TODO: 后面改成从配置表读取
            var levelScope = AssetSystem.Instance.GetScope(AssetsScopeLabel.Level);
            var assetHandle = levelScope.Acquire<ActorData>($"Level:ActorConfigs:Actor_{enemyID}");
            return assetHandle.Asset;
        }
    }
}