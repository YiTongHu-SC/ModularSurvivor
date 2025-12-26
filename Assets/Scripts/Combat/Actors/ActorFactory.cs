using System;
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
        private cfg.game.Tbconfig GameConfig => TableTool.Tables.Tbconfig;
        public void Initialize(RuntimeIdAllocator allocator)
        {
            Allocator = allocator;
        }

        internal void Reset()
        {
            Allocator = null;
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
            var table = TableTool.Tables.TbCharacter.Get(characterId);
            var targetPrefabKey = table.Prefab;
            var handle = AssetSystem.Instance.LevelScope.Acquire<GameObject>(targetPrefabKey);
            var targetPrefab = handle.Asset;
            var group = table.Group;
            var unitData = new UnitData(position, rotation)
            {
                Group = (GroupType)(int)group,
                MoveSpeed = table.MoveSpeed,
                MovementStrategy = table.Movement,
                ModelView = new UnitModelView(),
                CollisionData = new UnitCollisionData()
            };

            unitData.SetHealth(ActorAttributeUtils.GetMaxHp(table.BaseStrength, table.StrengthBonuses, 1));
            return Spawn(targetPrefab, unitData);
        }


    }
}