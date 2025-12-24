using LubanGenerated.TableTool;
using NUnit.Framework;

namespace Tests.ConfigData
{
    [TestFixture]
    public class LubanTablesTest
    {
        [Test]
        public void TestLubanTablesInitialization()
        {
            // 初始化表格工具
            TableTool.Initialize();
            var dataCout = TableTool.Tables.Tbitem.DataMap.Count;
            Assert.Greater(dataCout, 0, "配置表 Tbitem 应该包含数据");
        }
    }
}