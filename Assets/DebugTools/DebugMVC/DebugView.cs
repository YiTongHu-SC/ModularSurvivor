using System.Collections.Generic;
using cfg.game;
using Combat.Systems;
using Core.Events;
using Core.Units;
using DebugTools.Data;
using DebugTools.UI;
using Lean.Pool;
using TMPro;
using UI.Framework;
using UI.Utils;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace DebugTools.DebugMVC
{
    public class DebugView : BaseView<GameDebugData>
    {
        public DebugToolConfig Config;
        private TMP_InputField InputFieldDamageToHero;
        private Button ButtonTryDamageToHero;
        private TMP_InputField InputFieldSelectActorId;
        private Button ButtonSelectHeroActor;
        private Button ButtonCheckActorStatus;
        private Transform CurrentActorInfoContainer;
        private List<InfoShow> CurrentActorInfoShows = new();

        protected override void Awake()
        {
            base.Awake();
            UiTool.TryBind(transform, "InputFieldDamageToHero", out InputFieldDamageToHero);
            UiTool.TryBind(transform, "ButtonTryDamageToHero", out ButtonTryDamageToHero);
            UiTool.TryBind(transform, "InputFieldSelectActorId", out InputFieldSelectActorId);
            UiTool.TryBind(transform, "ButtonSelectHeroActor", out ButtonSelectHeroActor);
            UiTool.TryBind(transform, "ButtonCheckActorStatus", out ButtonCheckActorStatus);
            UiTool.TryBind(transform, "CurrentActorInfoContainer", out CurrentActorInfoContainer);
            ButtonTryDamageToHero.onClick.AddListener(TryDamageToHero);
            ButtonSelectHeroActor.onClick.AddListener(SelectHeroActor);
            ButtonCheckActorStatus.onClick.AddListener(CheckActorStatus);
        }

        private void Start()
        {
            ClearActorInfo();
            if (!Config)
            {
                Debug.LogError("DebugToolConfig is not assigned in DebugView.");
                return;
            }
            SelectHeroActor();
        }

        private void TryDamageToHero()
        {
            if (!UnitManager.Instance.IsInitialized) return;
            if (int.TryParse(InputFieldDamageToHero.text, out var damageAmount))
            {
                var heroId = UnitManager.Instance.HeroUnitData.RuntimeId;
                EventManager.Instance.Publish(new DebugEvents.ApplyDamageEvent(heroId, damageAmount));
            }
        }

        private void SelectHeroActor()
        {
            if (!UnitManager.Instance.IsInitialized) return;
            var actorId = UnitManager.Instance.HeroUnitData.RuntimeId;
            InputFieldSelectActorId.text = actorId.ToString();
        }

        private void CheckActorStatus()
        {
            if (!UnitManager.Instance.IsInitialized) return;
            if (!int.TryParse(InputFieldSelectActorId.text, out var actorId))
            {
                Debug.LogWarning("Please input actor id in damage input field to check actor status.");
                return;
            }
            Debug.Log($"Check actor {actorId}");
            EventManager.Instance.Publish(new DebugEvents.DebugActorEvent(DebugEvents.DebugActorAction.CheckActor, actorId.ToString()));
        }

        public override void UpdateView(GameDebugData data)
        {
            if (Config && data.IsShowActorInfo && data.SelectedActorId >= 0)
            {
                foreach (var infoShow in CurrentActorInfoShows)
                {
                    LeanPool.Despawn(infoShow.gameObject);
                }

                CurrentActorInfoShows.Clear();

                if (UnitManager.Instance.TryGetAvailableUnit(data.SelectedActorId, out var actor))
                {
                    CreateActorInfo("Actor Id", actor.RuntimeId.ToString());
                    CreateActorInfo("Actor Key", actor.Key);
                    CreateActorInfo("Health", $"{actor.Health}/{actor.MaxHealth}");
                    CreateActorInfo("Position", actor.Position.ToString());
                    CreateActorInfo("Group", actor.Group.ToString());
                    CreateActorInfo("Move Speed", actor.MoveSpeed.ToString());
                    // abilities
                    var actorAbilities = CombatManager.Instance.AbilitySystem.ActorAbilities;
                    if (actorAbilities.TryGetValue(actor.RuntimeId, out var abilities))
                    {
                        foreach (var ability in abilities)
                        {
                            CreateActorInfo($"Ability", $"Key:{ability.AbilityData.Key}");
                        }
                    }
                    else
                    {
                        CreateActorInfo("Abilities", "No abilities applied.");
                    }
                    // effects
                    var actorEffects = CombatManager.Instance.EffectSystem.UnitEffectNodes;
                    if (actorEffects.TryGetValue(actor.RuntimeId, out var effects))
                    {
                        foreach (var effect in effects)
                        {
                            CreateActorInfo($"Effect", $"Type:{effect.Type}");
                        }
                    }
                    else
                    {
                        CreateActorInfo("Effects", "No effects applied.");
                    }
                }
                else
                {
                    CreateActorInfo("Actor Info", $"No actor found with ID: {data.SelectedActorId}");
                }
            }
        }

        private InfoShow CreateActorInfo(string title, string info)
        {
            var infoShow = LeanPool.Spawn(Config.InfoShowPrefab, CurrentActorInfoContainer);
            infoShow.Init(title, info);
            CurrentActorInfoShows.Add(infoShow);
            return infoShow;
        }

        private void ClearActorInfo()
        {
            for (var i = CurrentActorInfoContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(CurrentActorInfoContainer.GetChild(i).gameObject);
            }
            CurrentActorInfoShows.Clear();
        }
    }
}