using Combat.Actors;
using Combat.Data;
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

        protected virtual Actor Spawn(ActorData actorData, UnitData unitData)
        {
            var actor = UnitManager.Instance.Factory.Spawn(actorData.ActorPrefab, unitData);
            return (Actor)actor;
        }

        public abstract void UpdateSpawner(float deltaTime);
    }
}