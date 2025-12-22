using Combat.Ability.Data;
using Combat.Data;
using Combat.Systems;
using Core.Assets;
using Core.Events;
using Core.Units;
using UnityEngine;

namespace GameLoop.GameDebug
{
    public class CreateHeroOnStart : MonoBehaviour
    {
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
                EventManager.Instance.Publish(new GameEvents.HeroCreated(heroData.GUID));
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
                CombatManager.Instance.AbilitySystem.ApplyAbility(TriggerType.LaserStrike, laserStrikeData,
                    heroData.GUID);
                // move ability
                var playerInputData = new PlayerInputData
                {
                    DeadZone = 0.1f,
                };
                CombatManager.Instance.AbilitySystem.ApplyAbility(TriggerType.PlayerInput, playerInputData,
                    heroData.GUID);

                // set actor as hero
                CombatManager.Instance.HeroActor = actor;
                CombatManager.Instance.CameraManager.BattleCameraController.SetTarget(
                    actor.transform,
                    heroData.ModelView.CenterOffset
                );
            }
            else
            {
                Debug.LogError("HeroPrefab is not assigned in CreateHeroOnStart.");
            }
        }
    }
}