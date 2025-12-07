using System.Collections.Generic;
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
        public UnitSystem UnitSystem { get; private set; }

        public Dictionary<int, UnitData> Units => UnitSystem.Units;

        public override void Initialize()
        {
            base.Initialize();
            UnitSystem = new UnitSystem();
            Factory = new UnitFactory();
        }

        public void Tick(float deltaTime)
        {
        }
    }
}