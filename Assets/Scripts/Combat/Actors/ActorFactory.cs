using Core.Units;
using Lean.Pool;
using Utils.Core;

namespace Combat.Actors
{
    public class ActorFactory
    {
        private RuntimeIdAllocator Allocator { get; set; }

        public void Initialize(RuntimeIdAllocator allocator)
        {
            Allocator = allocator;
        }

        public Actor Spawn(Actor actorPrefab, UnitData data)
        {
            data.RuntimeId = Allocator.Next();
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