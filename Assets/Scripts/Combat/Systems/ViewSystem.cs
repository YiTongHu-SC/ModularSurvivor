using Combat.Views;
using Core.Events;
using Core.Units;

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
            switch (eventData.ViewData.EventType)
            {
                case PresentationEventType.Add:
                    var view = ViewFactory.CreateUnitPresentation(eventData.PreferenceKey, eventData.ViewData);
                    view.Apply();
                    break;
            }
        }
    }
}