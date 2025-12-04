using UnityEngine;

namespace Core.Abstructs
{
    public abstract class BaseInstance<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance != null && Instance as MonoBehaviour != this)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this as T;
            DontDestroyOnLoad(this.gameObject);
        }

        public abstract void Initialize();
    }
}