using System.Collections.Generic;
using Combat.Ability.Data;
using Combat.Controller;
using Combat.Data;
using Combat.Effect;
using Combat.Systems;
using Core.Assets;
using Core.AssetsTool;
using Core.Events;
using Core.Units;
using UnityEngine;

namespace GameLoop.GameDebug
{
    public class CreateHeroOnStart : MonoBehaviour
    {
        public PlayerControllerConfig PlayerControllerConfig;
        public string HeroConfigKey = "DefaultHero";
        public Vector3 SpawnPositionOffset = Vector3.zero;

        private void Start()
        {
            CreateHero();
        }

        public void CreateHero()
        {
            var levelScope = AssetSystem.Instance.GetScope(AssetsScopeLabel.Level);
            var assetHandle = levelScope.Acquire<ActorData>(HeroConfigKey);
            if (assetHandle.Asset != null && assetHandle.Asset.ActorPrefab != null)
            {
                var heroData = new UnitData(SpawnPositionOffset + transform.position, 0)
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

                heroData.SetHealth(100);
                var actor = CombatManager.Instance.ActorFactory.Spawn(assetHandle.Asset.ActorPrefab, heroData);
                // add player controller, set target to hero
                var playerController = actor.gameObject.AddComponent<PlayerController>();
                playerController.Config = PlayerControllerConfig;
                playerController.SetTarget(heroData.RuntimeId);
                UnitManager.Instance.SetHeroUnit(heroData.RuntimeId);

                // Give the hero a Laser Strike ability for testing
                var laserStrikeData = new AbilityTriggerByIntervalData
                {
                    RuntimeID = CombatManager.Instance.GlobalAllocator.Next(),
                    Key = "HeroLaserStrike",
                    SourceId = heroData.RuntimeId,
                    TriggerType = TriggerType.Interval,
                    Interval = 1,
                    Cooldown = 0f,
                    FindTargetType = FindTargetType.Enemy,
                    EffectSpec = new EffectSpec()
                    {
                        Key = "DamageEffect",
                        EffectNodeType = EffectNodeType.DamageOnComplete,
                        Delay = 0,
                        Duration = 0.2f,
                        EffectParams = new Dictionary<string, object>()
                        {
                            { "DamageAmount", 20f },
                            {
                                "ViewData", new ViewBaseData()
                                {
                                    ViewEventType = ViewEventType.Add,
                                }
                            }
                        },
                        PreferenceKey = "Level:EffectConfigs:PresentationConfig",
                    },
                    ExtraParams = new Dictionary<string, object>()
                    {
                        {
                            "CollisionData", new UnitCollisionData()
                            {
                                AreaType = CollisionAreaType.Circle,
                                Radius = 5f,
                            }
                        }
                    }
                };

                CombatManager.Instance.AbilitySystem.ApplyAbility(laserStrikeData);
                // set actor as hero
                CombatManager.Instance.HeroActor = actor;
                // publish hero created event
                EventManager.Instance.Publish(new GameEvents.HeroCreated(heroData.RuntimeId, actor.transform));
            }
            else
            {
                Debug.LogError("HeroPrefab is not assigned in CreateHeroOnStart.");
            }
        }
    }
}