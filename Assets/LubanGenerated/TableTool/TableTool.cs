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
        public static bool IsInitialized => Tables != null;
        public static void Initialize()
        {
            // 初始化游戏配置
            if (IsInitialized)
            {
                return;
            }
            Tables = new cfg.Tables(TableLoader);
        }

        public static void ForceInitialize()
        {
            Clear();
            Initialize();
        }

        private static JSONNode TableLoader(string fileName)
        {
            // 实现表格加载逻辑
            fileName += ".json";
            return JSON.Parse(File.ReadAllText(Path.Combine(Application.dataPath, TablePath, fileName),
                System.Text.Encoding.UTF8));
        }

        public static void Clear()
        {
            Tables = null;
        }
    }
}