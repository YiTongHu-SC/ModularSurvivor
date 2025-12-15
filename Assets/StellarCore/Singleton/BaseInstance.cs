using UnityEngine;

namespace StellarCore.Singleton
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
            Debug.Log($"Instance of {typeof(T)} Awake");
        }

        public virtual void Initialize()
        {
            Debug.Log($"Instance of {typeof(T)} Initialized");
        }
    }
}