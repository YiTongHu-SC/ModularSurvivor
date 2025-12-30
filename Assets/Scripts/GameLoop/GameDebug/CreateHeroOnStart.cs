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
            CombatManager.Instance.HeroActor = actor;
            // publish hero created event
            EventManager.Instance.Publish(new GameEvents.HeroCreated(actor.UnitData.RuntimeId, actor.transform));
        }
    }
}