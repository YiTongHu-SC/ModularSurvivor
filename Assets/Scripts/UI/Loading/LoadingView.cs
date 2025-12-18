using TMPro;
using UI.Framework;

namespace UI.Loading
{
    public class LoadingView : BaseView<LoadingModelData>
    {
        public TextMeshProUGUI LoadingText;
        public TextMeshProUGUI LoadingProgressText;
        private ProgressBarUI progressBar;

        protected override void Awake()
        {
            base.Awake();
            progressBar = GetComponentInChildren<ProgressBarUI>();
        }

        public override void UpdateView(LoadingModelData data)
        {
            progressBar.SetProgress(data.Progress);
            LoadingText.text = data.Message;
            LoadingProgressText.text = (data.Progress).ToString("P0");
        }
    }
}