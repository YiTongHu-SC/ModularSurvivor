using UI.Framework;
using UnityEngine;

namespace UI.Menus
{
    [UILayer(UILayer.Window, "UI:Prefab:MainMenuView", blockInput: true, allowStack: true)]
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

        protected override void OnDispose()
        {
            Debug.Log("GameMenuController disposed.");
            Model.Dispose();
            Object.Destroy(View.gameObject);
        }
    }
}