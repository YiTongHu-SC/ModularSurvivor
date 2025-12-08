using Core.Units;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game
{
    public class HeroHpBar : MonoBehaviour
    {
        public Image BarImage;

        private void Update()
        {
            BarImage.fillAmount = GetHeroHpFraction();
        }

        private float GetHeroHpFraction()
        {
            if (UnitManager.Instance.HeroUnitData == null) return 1;
            return Mathf.Clamp(UnitManager.Instance.HeroUnitData.Health / UnitManager.Instance.HeroUnitData.MaxHealth,
                0f, 1f);
        }
    }
}