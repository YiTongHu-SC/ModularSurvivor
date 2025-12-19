using Core.Events;
using UI.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menus
{
    public class MainMenuView : BaseView<MainMenuModelData>
    {
        public Button ButtonStartGame;
        public Button ButtonExitGame;

        private void Start()
        {
            if (ButtonStartGame)
            {
                ButtonStartGame.onClick.AddListener(OnStartButtonClicked);
            }
            else
            {
                Debug.LogWarning("MainMenuView: ButtonStartGame is not assigned in the inspector!", this);
            }

            if (ButtonExitGame)
            {
                ButtonExitGame.onClick.AddListener(OnExitButtonClicked);
            }
            else
            {
                Debug.LogWarning("MainMenuView: ButtonExitGame is not assigned in the inspector!", this);
            }
        }

        private void OnExitButtonClicked()
        {
            EventManager.Instance.Publish(new GameLoopEvents.GameExitEvent());
        }

        private void OnStartButtonClicked()
        {
            EventManager.Instance.Publish(new GameLoopEvents.GameStartEvent(0));
        }

        public override void UpdateView(MainMenuModelData data)
        {
        }
    }
}