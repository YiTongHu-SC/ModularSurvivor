using System.IO;
using cfg;
using SimpleJSON;
using UnityEngine;

namespace LubanGenerated.TableTool
{
    public static class TableTool
    {
        const string TablePath = @"LubanGenerated/GenData";
        public static Tables Tables;

        public static void Initialize()
        {
            // 初始化游戏配置
            Tables = new cfg.Tables(TableLoader);
        }

        private static JSONNode TableLoader(string fileName)
        {
            // 实现表格加载逻辑
            fileName += ".json";
            return JSON.Parse(File.ReadAllText(Path.Combine(Application.dataPath, TablePath, fileName),
                System.Text.Encoding.UTF8));
        }
    }
}