using System.Collections.Generic;

namespace Core.Units
{
    public class UnitSystem
    {
        public Dictionary<int, UnitData> Units { get; } = new();

        public void RegisterUnit(UnitData data)
        {
            Units[data.RuntimeId] = data;
        }

        public void UnregisterUnit(int id)
        {
            Units.Remove(id);
        }
    }
}