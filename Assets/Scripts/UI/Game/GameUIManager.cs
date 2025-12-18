using Core.Events;
using Core.Input;
using UI.Framework;
using UnityEngine;

namespace UI.Game
{
    public class GameUIManager : MonoBehaviour
    {
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