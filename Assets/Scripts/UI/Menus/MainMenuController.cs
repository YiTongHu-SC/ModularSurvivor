using UI.Framework;
using UnityEngine;

namespace UI.Menus
{
    [UILayer(UILayer.Window, "ui:prefab:main_menu_view", blockInput: true, allowStack: true)]
    public class MainMenuController : BaseUIController<MainMenuModel, MainMenuView>
    {
        public override bool Initialize(GameObject targetView, object args = null)
        {
            if (IsInitialized) return true;
            var model = new MainMenuModel();
            var view = targetView.TryGetComponent(out MainMenuView viewComponent);
            if (!view) return false;
            viewComponent.BindModel(model);
            Initialize(model, viewComponent);
            Debug.Log("MainMenuController initialized.");
            return true;
        }
    }
}