using Core.Events;
using Core.Input;
using UI.Framework;
using UI.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game
{
    public class GameMenuView : BaseView<GameMenuModelData>
    {
        public Button ButtonContinueGame;
        public Button ButtonReturnToMainMenu;

        private void Start()
        {
            if (ButtonContinueGame)
            {
                ButtonContinueGame.onClick.AddListener(ContinueGame);
            }
            else
            {
                Debug.LogWarning("GameMenuView: ButtonContinueGame is not assigned in the inspector!", this);
            }

            if (ButtonContinueGame)
            {
                ButtonReturnToMainMenu.onClick.AddListener(OnReturnToMainMenuClicked);
            }
            else
            {
                Debug.LogWarning("GameMenuView: ButtonReturnToMainMenu is not assigned in the inspector!", this);
            }
        }

        private void OnReturnToMainMenuClicked()
        {
            EventManager.Instance.Publish(new GameLoopEvents.ReturnToMainMenuEvent());
        }

        private void ContinueGame()
        {
            EventManager.Instance.Publish(
                new InputEvents.InputContextChangedEvent(InputEvents.InputContext.ContinueGame));
        }

        public override void UpdateView(GameMenuModelData data)
        {
        }
    }
}