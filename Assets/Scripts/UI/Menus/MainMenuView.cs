using Core.Events;
using UI.Framework;
using UI.Utils;
using UnityEngine.UI;

namespace UI.Menus
{
    public class MainMenuView : BaseView<MainMenuModelData>
    {
        public Button ButtonStartGame;
        public Button ButtonExitGame;

        private void Start()
        {
            if (UiTool.TryBind<Button>(transform, "ButtonStartGame", out var startButton))
            {
                ButtonStartGame = startButton;
                ButtonStartGame.onClick.AddListener(OnStartButtonClicked);
            }

            if (UiTool.TryBind<Button>(transform, "ButtonExitGame", out var exitButton))
            {
                ButtonExitGame = exitButton;
                ButtonExitGame.onClick.AddListener(OnExitButtonClicked);
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