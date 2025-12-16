using System.Collections.Generic;
using Combat.Systems;
using StellarCore.Singleton;
using UnityEngine;

namespace Core.Units
{
    /// <summary>
    ///     管理器单例，负责单位的整体管理
    /// </summary>
    public class UnitManager : BaseInstance<UnitManager>
    {
        public UnitSystem UnitSystem { get; private set; }
        public UnitOverlapSystem OverlapSystem { get; private set; }
        public Dictionary<int, UnitData> Units => UnitSystem.Units;
        public UnitData HeroUnitData { get; private set; }

        public void SetHeroUnit(int unitId)
        {
            if (Units.ContainsKey(unitId))
                HeroUnitData = Units[unitId];
            else
                Debug.LogWarning($"Unit with ID {unitId} does not exist.");
        }

        public override void Initialize()
        {
            base.Initialize();
            UnitSystem = new UnitSystem();
            OverlapSystem = new UnitOverlapSystem(Units);
        }

        public void Tick(float deltaTime)
        {
            OverlapSystem.TickCheckOverlap(deltaTime);
        }

        public bool CheckUnitAvailability(int unitId)
        {
            return Units.ContainsKey(unitId) && Units[unitId].IsActive;
        }
    }
}