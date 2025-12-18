using UI.Framework;
using UnityEngine.UI;

namespace UI.Menus
{
    public class MainMenuView : BaseView<MainMenuModelData>
    {
        public Button StartButton;
        public Button ExitButton;

        protected override void InitializeView()
        {
            base.InitializeView();
        }

        public override void UpdateView(MainMenuModelData data)
        {
        }
    }
}