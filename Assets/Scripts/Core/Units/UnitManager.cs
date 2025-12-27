using System;
using System.Collections.Generic;
using Core.GameInterface;
using StellarCore.Singleton;
using UnityEngine;

namespace Core.Units
{
    /// <summary>
    ///     管理器单例，负责单位的整体管理
    /// </summary>
    public class UnitManager : BaseInstance<UnitManager>, IManager
    {
        public UnitSystem UnitSystem { get; private set; }
        public UnitOverlapSystem OverlapSystem { get; private set; }
        public Dictionary<int, UnitData> Units => UnitSystem.Units;
        public UnitData HeroUnitData { get; private set; }

        public bool IsInitialized { get; private set; }

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
            OverlapSystem = new UnitOverlapSystem(UnitSystem.Units);
            IsInitialized = true;
        }

        public void Reset()
        {
            UnitSystem.Reset();
            UnitSystem = null;
            OverlapSystem.Reset();
            OverlapSystem = null;
            HeroUnitData = null;
            IsInitialized = false;
        }

        public void Tick(float deltaTime)
        {
            UnitSystem.Tick(deltaTime);
            OverlapSystem.Tick(deltaTime);
        }

        public bool CheckUnitAvailability(int unitId)
        {
            return Units.ContainsKey(unitId) && Units[unitId].IsActive;
        }

        public bool TryGetAvailableUnit(int unitId, out UnitData unitData)
        {
            unitData = null;
            if (!Units.ContainsKey(unitId))
                return false;

            var data = Units[unitId];
            if (!data.IsActive)
                return false;

            unitData = data;
            return true;
        }
    }
}