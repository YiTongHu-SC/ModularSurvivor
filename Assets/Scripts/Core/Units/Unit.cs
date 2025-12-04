using Lean.Pool;
using UnityEngine;

namespace Core.Units
{
    /// <summary>
    /// 单位对象基类
    /// </summary>
    public abstract class Unit : MonoBehaviour, IPoolable
    {
        public UnitData UnitData;

        public void Initialize(UnitData data)
        {
            UnitData = data;
        }

        public void Reset()
        {
        }

        public void OnSpawn()
        {
        }

        public void OnDespawn()
        {
        }
    }
}