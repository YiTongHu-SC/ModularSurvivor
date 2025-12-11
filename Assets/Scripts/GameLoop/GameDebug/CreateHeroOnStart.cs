using Combat.Ability.Data;
using Combat.Actors;
using Combat.Data;
using Combat.Systems;
using Core.Events;
using Core.Units;
using UnityEngine;

namespace GameLoop.GameDebug
{
    public class CreateHeroOnStart : MonoBehaviour
    {
        public int HeroID = 100000;
        public Vector3 SpawnPosition = Vector3.zero;

        public void CreateHero()
        {
            var actorData = Resources.Load<ActorData>($"ActorConfigs/Actor_{HeroID}");
            if (actorData != null && actorData.ActorPrefab != null)
            {
                var heroData = new UnitData(SpawnPosition, 0)
                {
                    Group = GroupType.Ally,
                    ModelView = new UnitModelView()
                    {
                        Height = 1,
                        CenterOffset = 0.5f,
                        Radius = 0.5f
                    },
                    MovementStrategy = "SimpleMove",
                    MoveSpeed = 2f,
                };
                heroData.SetHealth(100);
                var actor = UnitManager.Instance.Factory.Spawn(actorData.ActorPrefab, heroData);
                EventManager.Instance.PublishEvent(new GameEvents.HeroCreated(heroData.GUID));
                UnitManager.Instance.SetHeroUnit(heroData.GUID);
                // Give the hero a Laser Strike ability for testing
                var laserStrikeData = new LaserStrikeData
                {
                    PresentationId = 0,
                    DamageAmount = 25,
                    HitDuration = 0.2f,
                    HitCooldown = 1f,
                    collisionData = new UnitCollisionData()
                    {
                        AreaType = CollisionAreaType.Circle,
                        Radius = 5f
                    },
                };
                CombatManager.Instance.AbilitySystem.ApplyAbility(AbilityType.LaserStrike, laserStrikeData,
                    heroData.GUID);
                // move ability
                var playerInputData = new PlayerInputData
                {
                    DeadZone = 0.1f,
                };
                CombatManager.Instance.AbilitySystem.ApplyAbility(AbilityType.PlayerInput, playerInputData,
                    heroData.GUID);

                // set actor as hero
                CombatManager.Instance.HeroActor = actor as Actor;
            }
            else
            {
                Debug.LogError("HeroPrefab is not assigned in CreateHeroOnStart.");
            }
        }
    }
}