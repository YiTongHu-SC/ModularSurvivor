using Core.AssetsTool;
using Core.Units;
using Lean.Pool;
using LubanGenerated.TableTool;
using UnityEngine;
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

        public Actor Spawn(GameObject actorPrefab, UnitData data)
        {
            data.RuntimeId = Allocator.Next();
            var unitInstance = LeanPool.Spawn(actorPrefab);
            var unit = unitInstance.GetComponent<Actor>();
            unit.Initialize(data);
            return unit;
        }

        public void Despawn(Actor unit)
        {
            LeanPool.Despawn(unit);
        }

        public Actor SpawnCharacter(string characterId, Vector2 position = default, float rotation = 0)
        {
            // var heroData = UnitDataFactory.CreateHeroData(heroId);
            var targetPrefabKey = TableTool.Tables.TbCharacter.Get(characterId).Prefab;
            var handle = AssetSystem.Instance.LevelScope.Acquire<GameObject>(targetPrefabKey);
            var targetPrefab = handle.Asset;
            var unitData = new UnitData(position, rotation)
            {
                Group = GroupType.Ally,
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
                MoveSpeed = 2f,
                MovementStrategy = "SimpleMove",
            };

            return Spawn(targetPrefab, unitData);
        }
    }
}