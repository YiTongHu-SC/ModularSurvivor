using UI.Framework;

namespace UI.Loading
{
    public class LoadingView : BaseView<LoadingModelData>
    {
        private ProgressBarUI progressBar;

        protected override void Awake()
        {
            base.Awake();
            progressBar = GetComponentInChildren<ProgressBarUI>();
        }

        public override void UpdateView(LoadingModelData data)
        {
            progressBar.SetProgress(data.Progress);
        }
    }
}