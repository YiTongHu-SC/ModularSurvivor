using Combat.Ability;
using Combat.Actors;
using Combat.Systems;
using Core.Events;
using Core.Units;
using UnityEngine;

namespace GameLoop.GameDebug
{
    public class CreateHeroOnStart : MonoBehaviour
    {
        public Actor HeroPrefab;
        public Vector3 SpawnPosition = Vector3.zero;

        public void CreateHero()
        {
            if (HeroPrefab != null)
            {
                var heroData = new UnitData(SpawnPosition, 0)
                {
                    Group = GroupType.Ally,
                    ModelView = new UnitModelView()
                    {
                        Height = 1,
                        CenterOffset = 0.5f,
                        Radius = 0.5f
                    }
                };
                heroData.SetHealth(100);
                UnitManager.Instance.Factory.Spawn(HeroPrefab, heroData);
                EventManager.Instance.PublishEvent(new GameEvents.HeroCreated(heroData.GUID));
                UnitManager.Instance.SetHeroUnit(heroData.GUID);
                var abilityData = new LaserStrikeData
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
                CombatManager.Instance.AbilitySystem.ApplyAbility(AbilityType.LaserStrike, abilityData, heroData.GUID);
            }
            else
            {
                Debug.LogError("HeroPrefab is not assigned in CreateHeroOnStart.");
            }
        }
    }
}