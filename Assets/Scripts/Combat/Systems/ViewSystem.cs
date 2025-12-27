using System;
using Combat.Views;
using Core.Events;
using Core.GameInterface;
using Core.Units;

namespace Combat.Systems
{
    public class ViewSystem : IEventListener<GameEvents.UpdatePreferenceEvent>, ISystem
    {
        public ViewFactory ViewFactory { get; set; }

        public void Initialize()
        {
            ViewFactory = new ViewFactory();
            EventManager.Instance.Subscribe<GameEvents.UpdatePreferenceEvent>(this);
        }

        public void Reset()
        {
            ViewFactory = null;
            EventManager.Instance.Unsubscribe<GameEvents.UpdatePreferenceEvent>(this);
        }

        public void OnEventReceived(GameEvents.UpdatePreferenceEvent eventData)
        {
            switch (eventData.ViewData.ViewEventType)
            {
                case ViewEventType.Add:
                    var view = ViewFactory.CreateUnitView(eventData.PreferenceKey, eventData.ViewData);
                    view.Apply();
                    break;
            }
        }

        public void Tick(float deltaTime)
        {
        }
    }
}