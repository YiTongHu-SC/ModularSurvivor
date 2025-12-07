using System.Collections.Generic;

namespace Core.Units
{
    public class UnitSystem
    {
        private Dictionary<int, UnitData> _units = new();
        public Dictionary<int, UnitData> Units => _units;

        public UnitSystem()
        {
        }

        public void RegisterUnit(UnitData data)
        {
            _units[data.GUID] = data;
        }

        public void UnregisterUnit(int id)
        {
            _units.Remove(id);
        }
    }
}