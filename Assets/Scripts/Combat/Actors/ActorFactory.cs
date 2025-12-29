using System;
using System.Collections.Generic;
using Combat.Ability.Data;
using Combat.Config;
using Combat.Effect;
using Combat.Systems;
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
            var unit = GameObjectFactory.Spawn(actorPrefab);
            unit.Initialize(data);
            return unit;
        }

        public Actor Spawn(GameObject actorPrefab, UnitData data)
        {
            data.RuntimeId = Allocator.Next();
            var unitInstance = GameObjectFactory.Spawn(actorPrefab);
            var unit = unitInstance.GetComponent<Actor>();
            unit.Initialize(data);
            return unit;
        }

        public void Despawn(Actor unit)
        {
            GameObjectFactory.Despawn(unit.gameObject);
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

            unitData.RuntimeId = Allocator.Next();
            unitData.Key = characterId;
            // set health
            unitData.SetHealth(ActorAttributeUtils.GetMaxHp(table.BaseStrength, table.StrengthBonuses, 1));
            // apply abilities
            foreach (var abilityKey in table.Abilities)
            {
                var abilityConfig = AssetSystem.Instance.LevelScope.Acquire<ActorAbilityConfig>(abilityKey).Asset;
                var abilityData = GetAbilityDataFromConfig(abilityConfig, unitData.RuntimeId);
                CombatManager.Instance.AbilitySystem.ApplyAbility(abilityData);
            }
            var unitInstance = GameObjectFactory.Spawn(targetPrefab);
            var unit = unitInstance.GetComponent<Actor>();
            unit.Initialize(unitData);
            return unit;
        }

        #region Ability Parsing
        public AbilityData GetAbilityDataFromConfig(ActorAbilityConfig config, int sourceId)
        {
            AbilityData abilityData;
            // create ability data based on trigger type
            switch (config.TriggerType)
            {
                case TriggerType.Once:
                    // TODO: support more once types later
                    abilityData = new AbilityData
                    {
                        RuntimeID = Allocator.Next(),
                        Key = config.Name,
                        SourceId = sourceId,
                        FindTargetType = config.FindTargetType,
                        TriggerType = TriggerType.Once,
                        Cooldown = config.Cooldown,
                        Cost = config.Cost,
                        ExtraParams = new Dictionary<string, object>(),
                    };
                    break;
                case TriggerType.Interval:
                    abilityData = new AbilityTriggerByIntervalData
                    {
                        RuntimeID = Allocator.Next(),
                        Key = config.Name,
                        SourceId = sourceId,
                        FindTargetType = config.FindTargetType,
                        TriggerType = TriggerType.Interval,
                        Interval = config.Interval,
                        Cooldown = config.Cooldown,
                        Cost = config.Cost,
                        ExtraParams = new Dictionary<string, object>(),
                    };
                    break;
                case TriggerType.ByEvent:
                    // TODO: support more event types later
                    abilityData = new AbilityTriggerByEventData
                    {
                        RuntimeID = Allocator.Next(),
                        Key = config.Name,
                        SourceId = sourceId,
                        FindTargetType = config.FindTargetType,
                        TriggerType = TriggerType.ByEvent,
                        Cooldown = config.Cooldown,
                        Cost = config.Cost,
                        ExtraParams = new Dictionary<string, object>(),
                    };
                    break;
                default:
                    throw new ArgumentException($"Unsupported trigger type: {config.TriggerType}");
            }
            // set ability params
            ParseEffectParams(abilityData.ExtraParams, config.ExtraParams);
            // set effect data
            abilityData.EffectSpec = GetEffectSpecFromConfig(config.EffectConfig);
            return abilityData;
        }

        public EffectSpec GetEffectSpecFromConfig(EffectConfig config)
        {
            var effectSpec = new EffectSpec
            {
                Key = config.Key,
                PreferenceKey = config.PreferenceKey,
                EffectNodeType = config.EffectNodeType,
                Delay = config.Delay,
                Duration = config.Duration,
                EffectParams = new Dictionary<string, object>(),
            };
            // set effect params
            ParseEffectParams(effectSpec.EffectParams, config.ExtraParams);
            for (int i = 0; i < config.Children.Count; i++)
            {
                var childConfig = config.Children[i];
                var childEffectSpec = GetEffectSpecFromConfig(childConfig);
                effectSpec.children ??= new List<EffectSpec>();
                effectSpec.children.Add(childEffectSpec);
            }
            return effectSpec;
        }

        public void ParseEffectParams(Dictionary<string, object> extraParams, List<ParamData> paramDatas)
        {
            foreach (var param in paramDatas)
            {
                switch (param.ParamType)
                {
                    case ParamType.DamageAmount:
                        extraParams["DamageAmount"] = float.Parse(param.Value);
                        break;
                    case ParamType.HealAmount:
                        extraParams["HealAmount"] = float.Parse(param.Value);
                        break;
                    case ParamType.CollisionArea:
                        extraParams["CollisionData"] = new UnitCollisionData()
                        {
                            AreaType = (CollisionAreaType)Enum.Parse(typeof(CollisionAreaType), param.Value),
                        };
                        break;
                    case ParamType.CollisionRadius:
                        if (extraParams.ContainsKey("CollisionData"))
                        {
                            var collisionData = (UnitCollisionData)extraParams["CollisionData"];
                            collisionData.Radius = float.Parse(param.Value);
                            extraParams["CollisionData"] = collisionData;
                        }
                        break;
                    case ParamType.ViewEventType:
                        extraParams["ViewData"] = new ViewBaseData()
                        {
                            ViewEventType = (ViewEventType)Enum.Parse(typeof(ViewEventType), param.Value),
                        };
                        break;
                }
            }
        }
        #endregion
    }
}