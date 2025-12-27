using System;
using System.Collections.Generic;
using Core.GameInterface;

namespace Core.Units
{
    public class UnitSystem : ISystem
    {
        public Dictionary<int, UnitData> Units { get; } = new();

        public void RegisterUnit(UnitData data)
        {
            Units[data.RuntimeId] = data;
        }

        public void Tick(float deltaTime)
        {
        }

        public void UnregisterUnit(int id)
        {
            Units.Remove(id);
        }

        public void Reset()
        {
            Units.Clear();
        }
    }
}