using Core.Events;
using UI.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace DebugTools.DebugMVC
{
    public class DebugView : BaseView<GameDebugData>
    {
        [SerializeField] private Button StartGameButton;
        [SerializeField] private Button ReturnMainButton;
        [SerializeField] private Button ExitGameButton;

        protected override void Awake()
        {
            base.Awake();
            StartGameButton.onClick.AddListener(OnStartGameClicked);
            ReturnMainButton.onClick.AddListener(OnReturnMainClicked);
            ExitGameButton.onClick.AddListener(OnExitGameClicked);
        }

        private void OnExitGameClicked()
        {
            EventManager.Instance.Publish(new GameEvents.GameExitEvent());
        }

        private void OnReturnMainClicked()
        {
            EventManager.Instance.Publish(new GameEvents.ReturnToMainMenuEvent());
        }

        private void OnStartGameClicked()
        {
            EventManager.Instance.Publish(new GameEvents.GameStartEvent(0));
        }

        public override void UpdateView(GameDebugData data)
        {
        }
    }
}