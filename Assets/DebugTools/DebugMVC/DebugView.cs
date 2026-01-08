using System;
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
        private Button ButtonLoopActorSelect;
        private Transform CurrentActorInfoContainer;
        private List<InfoShow> CurrentActorInfoShows = new();
        private int _currentLoopActorIndex = 0;
        private TMP_InputField InputFieldSetHeroSightRange;
        private Button ButtonSetHeroSightRange;

        protected override void Awake()
        {
            base.Awake();
            UiTool.TryBind(transform, "InputFieldDamageToHero", out InputFieldDamageToHero);
            UiTool.TryBind(transform, "ButtonTryDamageToHero", out ButtonTryDamageToHero);
            UiTool.TryBind(transform, "InputFieldSelectActorId", out InputFieldSelectActorId);
            UiTool.TryBind(transform, "ButtonSelectHeroActor", out ButtonSelectHeroActor);
            UiTool.TryBind(transform, "ButtonCheckActorStatus", out ButtonCheckActorStatus);
            UiTool.TryBind(transform, "CurrentActorInfoContainer", out CurrentActorInfoContainer);
            UiTool.TryBind(transform, "ButtonLoopActorSelect", out ButtonLoopActorSelect);
            UiTool.TryBind(transform, "InputFieldSetHeroSightRange", out InputFieldSetHeroSightRange);
            UiTool.TryBind(transform, "ButtonSetHeroSightRange", out ButtonSetHeroSightRange);
            ButtonTryDamageToHero.onClick.AddListener(TryDamageToHero);
            ButtonSelectHeroActor.onClick.AddListener(SelectHeroActor);
            ButtonCheckActorStatus.onClick.AddListener(CheckActorStatus);
            ButtonLoopActorSelect.onClick.AddListener(LoopActorSelect);
            ButtonSetHeroSightRange.onClick.AddListener(SetHeroSightRange);
        }

        /// <summary>
        /// Set hero sight range based on input field value.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void SetHeroSightRange()
        {
            if (!UnitManager.Instance.IsInitialized) return;
            if (float.TryParse(InputFieldSetHeroSightRange.text, out var sightRange))
            {
                Debug.Log($"Set hero sight range to {sightRange}");
                EventManager.Instance.Publish(new DebugEvents.DebugActorEvent(DebugEvents.DebugActorAction.SetHeroSightRange, sightRange.ToString()));
            }
        }

        /// <summary>
        /// Loop select actor for testing purpose.
        /// </summary>
        private void LoopActorSelect()
        {
            if (!UnitManager.Instance.IsInitialized) return;
            var count = 0;
            foreach (var unit in UnitManager.Instance.Units.Keys)
            {
                if (count == _currentLoopActorIndex)
                {
                    InputFieldSelectActorId.text = unit.ToString();
                    CheckActorStatus();
                    _currentLoopActorIndex = (_currentLoopActorIndex + 1) % UnitManager.Instance.Units.Count;
                    break;
                }
                count++;
            }
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
                            CreateActorInfo($"Effect", $"Type:{effect.EffectType}");
                        }
                    }
                    else
                    {
                        CreateActorInfo("Effects", "No effects applied.");
                    }
                    // buffs
                    var actorBuffs = CombatManager.Instance.BuffSystem.UnitBuffs;
                    if (actorBuffs.TryGetValue(actor.RuntimeId, out var buffs))
                    {
                        foreach (var buff in buffs)
                        {
                            CreateActorInfo($"Buff", $"{buff.Data.Name}");
                        }
                    }
                    else
                    {
                        CreateActorInfo("Buffs", "No buffs applied.");
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