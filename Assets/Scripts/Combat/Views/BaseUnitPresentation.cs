using Core.Units;
using Lean.Pool;
using UnityEngine;

namespace Combat.Views
{
    public abstract class BaseUnitPresentation : MonoBehaviour, IPoolable
    {
        public PresentationConfig Config { get; private set; }
        public ViewBaseData ViewData { get; private set; }

        public virtual void SetConfig(PresentationConfig config)
        {
            Config = config;
        }

        public virtual void SetViewData(ViewBaseData viewData)
        {
            ViewData = viewData;
        }

        public abstract void Apply();

        public virtual void OnSpawn()
        {
        }

        public virtual void OnDespawn()
        {
        }
    }
}