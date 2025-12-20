using System.Collections.Generic;
using Combat.Systems;
using Core.Units;
using NUnit.Framework;

namespace Tests.Unit
{
    [TestFixture]
    public class OverlapSystemTest
    {
        private Dictionary<int, UnitData> _allUnits;
        private UnitOverlapSystem _unitOverlapSystem;

        [SetUp]
        public void Setup()
        {
            _allUnits = new Dictionary<int, UnitData>();
            _unitOverlapSystem = new UnitOverlapSystem(_allUnits);
        }
    }
}