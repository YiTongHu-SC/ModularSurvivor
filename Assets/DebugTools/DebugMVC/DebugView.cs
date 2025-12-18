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
            throw new System.NotImplementedException();
        }

        private void OnReturnMainClicked()
        {
            throw new System.NotImplementedException();
        }

        private void OnStartGameClicked()
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateView(GameDebugData data)
        {
        }
    }
}