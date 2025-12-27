using Combat.Systems;
using UI.Framework;
using UnityEngine;

namespace UI.Game
{
    [UILayer(UILayer.HUD, "UI:Prefab:LevelHudView", blockInput: true, allowStack: true)]
    public class LevelHudController : BaseUIController<LevelHudModel, LevelHudView>
    {
        public override bool Initialize(GameObject targetView, object args = null)
        {
            if (IsInitialized)
            {
                Debug.LogWarning("LevelHudController: Controller already initialized!");
                return false;
            }

            if (targetView == null)
            {
                Debug.LogError("LevelHudController: Target view is null!");
                return false;
            }

            if (targetView.TryGetComponent<LevelHudView>(out var view))
            {
                var model = new LevelHudModel();
                view.BindModel(model);
                Initialize(model, view);
                return true;
            }
            else
            {
                Debug.LogError("LevelHudController: Target view does not have LevelHudView component!");
                return false;
            }
        }


        public void Update()
        {
            if (!IsInitialized || !IsOpen) return;

            // Example logic to update the battle clock
            var hudData = new LevelHudModelData()
            {
                BattleClock = CombatManager.Instance.CombatClock.BattleClock,
                MaxBattleTime = CombatManager.Instance.CombatClock.MaxBattleTime,
            };

            Model.SetValue(hudData);
        }

        protected override void OnDispose()
        {
            Debug.Log("LevelHudController: Disposing!");
            Model.Dispose();
            Object.Destroy(View.gameObject);
        }
    }
}