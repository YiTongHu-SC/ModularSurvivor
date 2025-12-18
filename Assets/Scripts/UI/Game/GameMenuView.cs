using Core.Events;
using Core.Input;
using UI.Framework;
using UI.Utils;
using UnityEngine.UI;

namespace UI.Game
{
    public class GameMenuView : BaseView<GameMenuModelData>
    {
        public Button ButtonContinueGame;
        public Button ButtonReturnToMainMenu;

        private void Start()
        {
            if (UiTool.TryBind<Button>(transform, "ButtonContinueGame", out var btnContinueGame))
            {
                ButtonContinueGame = btnContinueGame;
                ButtonContinueGame.onClick.AddListener(ContinueGame);
            }

            if (UiTool.TryBind<Button>(transform, "ButtonReturnToMainMenu", out var btnReturnMain))
            {
                ButtonReturnToMainMenu = btnReturnMain;
                ButtonReturnToMainMenu.onClick.AddListener(OnReturnToMainMenuClicked);
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