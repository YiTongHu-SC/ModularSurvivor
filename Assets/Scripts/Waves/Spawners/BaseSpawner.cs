using Combat.Actors;
using Combat.Data;
using Combat.Systems;
using Core.Units;
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
            var actor = CombatManager.Instance.ActorFactory.Spawn(actorData.ActorPrefab, unitData);
            return actor;
        }

        public abstract void UpdateSpawner(float deltaTime);
    }
}