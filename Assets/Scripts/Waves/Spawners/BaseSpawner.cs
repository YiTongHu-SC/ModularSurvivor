using Combat.Actors;
using Combat.Data;
using Combat.Systems;
using Core.Units;
using UnityEngine;
using Waves.Data;

namespace Waves.Spawners
{
    public abstract class BaseSpawner
    {
        public WaveConfig Config;

        protected BaseSpawner(WaveConfig config)
        {
            Config = config;
        }

        protected virtual Actor Spawn(GameObject actorPrefab, UnitData unitData)
        {
            return CombatManager.Instance.ActorFactory.Spawn(actorPrefab, unitData);
        }

        public abstract void UpdateSpawner(float deltaTime);
    }
}