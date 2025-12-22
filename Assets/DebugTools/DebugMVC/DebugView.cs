using Core.Events;
using Core.Units;
using TMPro;
using UI.Framework;
using UI.Utils;
using UnityEngine.UI;

namespace DebugTools.DebugMVC
{
    public class DebugView : BaseView<GameDebugData>
    {
        private TMP_InputField InputFieldDamageToHero;
        private Button ButtonTryDamageToHero;

        protected override void Awake()
        {
            base.Awake();
            UiTool.TryBind(transform, "ButtonTryDamageToHero", out ButtonTryDamageToHero);
            UiTool.TryBind(transform, "InputFieldDamageToHero", out InputFieldDamageToHero);
            ButtonTryDamageToHero.onClick.AddListener(OnClickTryDamageToHero);
        }

        private void OnClickTryDamageToHero()
        {
            if (int.TryParse(InputFieldDamageToHero.text, out var damageAmount))
            {
                var heroId = UnitManager.Instance.HeroUnitData.RuntimeId;
                EventManager.Instance.Publish(new DebugEvents.ApplyDamageEvent(heroId, damageAmount));
            }
        }

        public override void UpdateView(GameDebugData data)
        {
        }
    }
}