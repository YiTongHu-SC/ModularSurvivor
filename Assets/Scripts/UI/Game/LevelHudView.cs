using TMPro;
using UI.Framework;

namespace UI.Game
{
    public class LevelHudView : BaseView<LevelHudModelData>
    {
        public TextMeshProUGUI ClockText;

        public override void UpdateView(LevelHudModelData data)
        {
            if (!IsInitialized) return;
            if (!ClockText) return;
            float remainingTime = data.MaxBattleTime - data.BattleClock;
            if (remainingTime < 0f) remainingTime = 0f;

            int minutes = (int)(remainingTime / 60f);
            int seconds = (int)(remainingTime % 60f);

            ClockText.text = $"{minutes:00}:{seconds:00}";
        }
    }
}