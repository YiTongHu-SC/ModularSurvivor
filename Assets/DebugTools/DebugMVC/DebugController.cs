using System.Collections.Generic;
using Combat.Ability;
using Combat.Effect;
using Core.Events;
using UI.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DebugTools.DebugMVC
{
    [UILayer(UILayer.System, "Debug:UI:DebugView", blockInput: false, allowStack: false)]
    public class DebugController : BaseUIController<DebugModel, DebugView>
    {
        public override bool Initialize(GameObject targetView, object args = null)
        {
            if (IsInitialized) return true;
            var model = new DebugModel();
            var view = targetView.TryGetComponent(out DebugView viewComponent);
            if (!view) return false;
            viewComponent.BindModel(model);
            Initialize(model, viewComponent);
            SubscribeEvents();
            return true;
        }

        protected override void OnDispose()
        {
            UnsubscribeEvents();
            Model.Dispose();
            Object.Destroy(View.gameObject);
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            EventManager.Instance.Subscribe<DebugEvents.ApplyDamageEvent>(ApplyDamage);
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
            EventManager.Instance.Unsubscribe<DebugEvents.ApplyDamageEvent>(ApplyDamage);
        }

        private void ApplyDamage(DebugEvents.ApplyDamageEvent obj)
        {
            var effectSpec = new EffectSpec()
            {
                EffectNodeType = EffectNodeType.Damage,
                EffectParams = new Dictionary<string, object>
                {
                    { "DamageAmount", 10f },
                }
            };

            var effect = EffectFactory.CreateEffectNode(effectSpec);
            effect.SetContext(new AbilityContext(-1)
            {
                SourceId = 1,
                Targets = new TargetSet
                {
                    TargetUnits = { obj.TargetId }
                },
            });
            effect.Execute();
            MvcManager.Instance.Close<DebugController>();
        }
    }
}