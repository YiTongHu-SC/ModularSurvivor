using LubanGenerated.TableTool;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Tables
{
    [TestFixture]
    public class TableReadTest
    {
        [SetUp]
        public void Setup()
        {
            // Setup code if needed
            TableTool.Initialize();
        }

        [Test]
        public void ItemTableTest()
        {
            var itemTable = TableTool.Tables.TbItem;
            Assert.IsNotNull(itemTable, "Item table should not be null");
            var itemCount = itemTable.DataList.Count;
            Assert.IsTrue(itemCount > 0, "Item table should contain items");
        }

        [Test]
        public void HeroLevelTableTest()
        {
            var heroLevelTable = TableTool.Tables.TbHeroLevel;
            Assert.IsNotNull(heroLevelTable, "HeroLevel table should not be null");
            var levelCount = heroLevelTable.DataList.Count;
            Assert.IsTrue(levelCount > 0, "HeroLevel table should contain levels");
            var data = heroLevelTable.Get("actor_hero_1", 1);
            Assert.IsNotNull(data, "HeroLevel data for actor_hero_1 at level 1 should not be null");
            Debug.Log($"HeroLevel Data: ActorKey={data.Key}, Level={data.Level}, ExpRequired={data.Exp}");
        }
    }
}