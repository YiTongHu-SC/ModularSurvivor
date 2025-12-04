using UnityEngine;

namespace Core.Units
{
    /// <summary>
    /// 管理器单例，负责单位的整体管理
    /// </summary>
    public class UnitManager : MonoBehaviour
    {
        public static UnitManager Instance { get; private set; }
        public UnitFactory Factory { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            Initialize();
        }

        public void Initialize()
        {
            Factory = new UnitFactory();
        }
    }
}