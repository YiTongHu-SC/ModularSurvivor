using System.Collections;
using Core.Units;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Core
{
    public class TestScriptUnitManager
    {
        // A Test behaves as an ordinary method
        [Test]
        public void NewTestScriptSimplePasses()
        {
            // Use the Assert class to test conditions
            var unitData = new UnitData(new Vector2(0, 0), 100);
            Assert.IsNotNull(unitData);
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator NewTestScriptWithEnumeratorPasses()
        {
            // 创建 GameObject 并添加 UnitManager 组件
            GameObject unitManagerGO = new GameObject("UnitManager");
            UnitManager unitManager = unitManagerGO.AddComponent<UnitManager>();
            
            // 等待一帧以确保 Awake 被调用
            yield return null;

            // 验证单例实例已正确设置
            Assert.IsNotNull(UnitManager.Instance);
            Assert.AreEqual(unitManager, UnitManager.Instance);
            
            // 验证 Factory 已被初始化
            Assert.IsNotNull(UnitManager.Instance.Factory);

            // 清理测试对象
            Object.DestroyImmediate(unitManagerGO);
        }

        [UnityTest]
        public IEnumerator TestSingletonBehavior()
        {
            // 创建第一个 UnitManager
            GameObject firstGO = new GameObject("UnitManager1");
            UnitManager firstManager = firstGO.AddComponent<UnitManager>();
            
            yield return null;
            
            // 验证第一个实例正确设置
            Assert.IsNotNull(UnitManager.Instance);
            Assert.AreEqual(firstManager, UnitManager.Instance);
            
            // 尝试创建第二个 UnitManager
            GameObject secondGO = new GameObject("UnitManager2");
            secondGO.AddComponent<UnitManager>();
            
            yield return null;
            
            // 验证单例行为 - 只有第一个实例存在
            Assert.AreEqual(firstManager, UnitManager.Instance);
            
            // 验证第二个 GameObject 被销毁了（单例行为）
            // 注意：这个测试可能需要根据具体的销毁逻辑调整
            
            // 清理
            if (firstGO != null)
                Object.DestroyImmediate(firstGO);
            if (secondGO != null)
                Object.DestroyImmediate(secondGO);
        }
    }
}