using Core.Abstructs;
using UnityEngine;

namespace Core.Units
{
    /// <summary>
    /// 管理器单例，负责单位的整体管理
    /// </summary>
    public class UnitManager : BaseInstance<UnitManager>
    {
        public UnitFactory Factory { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            if (Instance == this)
            {
                Initialize();
            }
        }

        public override void Initialize()
        {
            Factory = new UnitFactory();
        }
    }
}