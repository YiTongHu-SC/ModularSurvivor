using Combat.Systems;
using Core.Events;
using Core.Input;
using UI.Framework;
using UnityEngine;

namespace UI.Game
{
    public class GameUIManager : MonoBehaviour
    {
        private LevelHudController _levelHudController;
        private HeroHealthController _heroHealthController;

        private void Start()
        {
            MvcManager.Instance.Open<LevelHudController>();
            MvcManager.Instance.Open<HeroHealthController>();
            _levelHudController = MvcManager.Instance.GetUIController<LevelHudController>();
            _heroHealthController = MvcManager.Instance.GetUIController<HeroHealthController>();
        }

        private void Update()
        {
            _levelHudController.Update();
            _heroHealthController.Update();
        }

        private void OnEnable()
        {
            EventManager.Instance.Subscribe<InputEvents.InputContextChangedEvent>(ProcessEvent);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe<InputEvents.InputContextChangedEvent>(ProcessEvent);
        }

        private void ProcessEvent(InputEvents.InputContextChangedEvent evt)
        {
            switch (evt.Context)
            {
                case InputEvents.InputContext.Paused:
                    OpenMenu();
                    break;
                case InputEvents.InputContext.ContinueGame:
                    CloseMenu();
                    break;
            }
        }

        private void OnDestroy()
        {
            MvcManager.Instance.DisposeUI<GameMenuController>();
            MvcManager.Instance.DisposeUI<LevelHudController>();
            MvcManager.Instance.DisposeUI<HeroHealthController>();
        }

        private void CloseMenu()
        {
            MvcManager.Instance.Close<GameMenuController>();
        }

        private void OpenMenu()
        {
            MvcManager.Instance.Open<GameMenuController>();
        }
    }
}