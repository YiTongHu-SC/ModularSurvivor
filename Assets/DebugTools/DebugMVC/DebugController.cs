using UI.Framework;
using UnityEngine;

namespace DebugTools.DebugMVC
{
    [UILayer(UILayer.System, "debug:ui:debug_view", blockInput: false, allowStack: false)]
    public class DebugController : BaseUIController<DebugModel, DebugView>
    {
        public override bool Initialize(GameObject targetView, object args = null)
        {
            if (IsInitialized) return true;
            var model = new DebugModel();
            var view = targetView.TryGetComponent(out DebugView viewComponent);
            if (!view) return false;
            viewComponent.BindModel(model);
            Initialize(model, viewComponent);
            return true;
        }
    }
}