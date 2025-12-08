using System;
using Core.Events;
using Core.Units;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game
{
    public class HeroHpBar : MonoBehaviour, IEventListener<GameEvents.HeroCreated>
    {
        public Image BarImage;
        private int targetHeroId = -1;
        private UnitData HeroData { get; set; }

        public void OnEventReceived(GameEvents.HeroCreated eventData)
        {
            targetHeroId = eventData.UnitGuid;
            HeroData = UnitManager.Instance.Units[targetHeroId];
        }

        private void Update()
        {
            BarImage.fillAmount = GetHeroHpFraction();
        }

        private float GetHeroHpFraction()
        {
            if (HeroData == null) return 1;
            return Mathf.Clamp(HeroData.Health / HeroData.MaxHealth, 0f, 1f);
        }
    }
}