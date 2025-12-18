using Core.Events;
using UI.Framework;

namespace UI.Loading
{
    public class LoadingController : BaseController<LoadingModel, LoadingView>
    {
        protected override void OnInitialize()
        {
            base.OnInitialize();
            View.BindModel(Model);
            EventManager.Instance.Subscribe<GameLoopEvents.LoadingProgressEvent>(UpdateLoading);
        }

        private void UpdateLoading(GameLoopEvents.LoadingProgressEvent evt)
        {
            var data = new LoadingModelData
            {
                Progress = evt.Progress,
                Message = evt.Message
            };
            Model.SetValue(data);
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            EventManager.Instance.Unsubscribe<GameLoopEvents.LoadingProgressEvent>(UpdateLoading);
        }
    }
}