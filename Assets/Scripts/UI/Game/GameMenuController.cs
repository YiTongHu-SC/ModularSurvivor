using UI.Framework;
using UnityEngine;

namespace UI.Game
{
    [UILayer(UILayer.Window, "ui:prefab:game_menu_view", blockInput: true, allowStack: true)]
    public class GameMenuController : BaseUIController<GameMenuModel, GameMenuView>
    {
        public override bool Initialize(GameObject targetView, object args = null)
        {
            if (IsInitialized) return true;
            var model = new GameMenuModel();
            var view = targetView.TryGetComponent(out GameMenuView viewComponent);
            if (!view) return false;
            viewComponent.BindModel(model);
            Initialize(model, viewComponent);
            Debug.Log("GameMenuController initialized.");
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