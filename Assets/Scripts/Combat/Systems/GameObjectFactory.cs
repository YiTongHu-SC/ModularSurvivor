using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

namespace Combat.Systems
{
    /// <summary>
    /// 工厂类，用于创建各种类型的GameObject
    /// </summary>
    public static class GameObjectFactory
    {
        private static HashSet<GameObject> _createdObjects = new();

        public static T Spawn<T>(T prefab) where T : Component
        {
            var go = LeanPool.Spawn<T>(prefab);
            _createdObjects.Add(go.gameObject);
            return go;
        }

        public static GameObject Spawn(GameObject prefab)
        {
            var go = LeanPool.Spawn(prefab);
            _createdObjects.Add(go);
            return go;
        }

        public static void Despawn(GameObject obj)
        {
            LeanPool.Despawn(obj);
        }

        public static void Cleanup()
        {
            foreach (var obj in _createdObjects)
            {
                if (obj != null)
                {
                    LeanPool.Despawn(obj);
                    LeanPool.Detach(obj, true);
                    GameObject.Destroy(obj);
                }
            }
            _createdObjects.Clear();
        }
    }
}