using System.Collections.Generic;
using Combat.Ability.Data;
using Combat.Controller;
using Combat.Effect;
using Combat.Systems;
using Core.Events;
using Core.Units;
using UnityEngine;

namespace GameLoop.GameDebug
{
    public class CreateHeroOnStart : MonoBehaviour
    {
        public string HeroKey = "DefaultHero";
        public PlayerControllerConfig PlayerControllerConfig;
        public Vector3 SpawnPositionOffset = Vector3.zero;

        private void Start()
        {
            CreateHero();
        }

        public void CreateHero()
        {
            // heroData.SetHealth(100);
            // var actor = CombatManager.Instance.ActorFactory.Spawn(assetHandle.Asset.ActorPrefab, heroData);
            // add player controller, set target to hero

            var actor = CombatManager.Instance.ActorFactory.SpawnCharacter(HeroKey);
            var playerController = actor.gameObject.AddComponent<PlayerController>();
            playerController.Config = PlayerControllerConfig;
            playerController.SetTarget(actor.UnitData.RuntimeId);
            UnitManager.Instance.SetHeroUnit(actor.UnitData.RuntimeId);

            // Give the hero a Laser Strike ability for testing
            var laserStrikeData = new AbilityTriggerByIntervalData
            {
                RuntimeID = CombatManager.Instance.GlobalAllocator.Next(),
                Key = "HeroLaserStrike",
                SourceId = actor.UnitData.RuntimeId,
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
            EventManager.Instance.Publish(new GameEvents.HeroCreated(actor.UnitData.RuntimeId, actor.transform));
        }
    }
}