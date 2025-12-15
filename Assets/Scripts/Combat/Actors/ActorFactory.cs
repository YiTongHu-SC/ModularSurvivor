using Core.Units;
using Lean.Pool;

namespace Combat.Actors
{
    public class ActorFactory
    {
        private int _uuid = 0;

        public Actor Spawn(Actor actorPrefab, UnitData data)
        {
            _uuid++;
            data.GUID = _uuid;
            var unit = LeanPool.Spawn(actorPrefab);
            unit.Initialize(data);
            return unit;
        }

        public void Despawn(Actor unit)
        {
            LeanPool.Despawn(unit);
        }
    }
}