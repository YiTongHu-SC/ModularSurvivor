using Combat.Views;
using Core.Events;

namespace Combat.Systems
{
    public class ViewSystem : IEventListener<GameEvents.UpdatePreferenceEvent>
    {
        public ViewFactory ViewFactory { get; set; }

        public void Initialize()
        {
            ViewFactory = new ViewFactory();
            EventManager.Instance.Subscribe<GameEvents.UpdatePreferenceEvent>(this);
        }

        public void OnEventReceived(GameEvents.UpdatePreferenceEvent eventData)
        {
            throw new System.NotImplementedException();
        }
    }
}