using UnityEngine;

namespace Waves.Data
{
    /// <summary>
    /// 单个波次配置数据
    /// </summary>
    [CreateAssetMenu(fileName = "WaveConfig", menuName = "Wave Config/WaveConfig", order = 0)]
    public class WaveConfig : ScriptableObject
    {
        /// <summary>
        /// 敌人ID
        /// </summary>
        public int EnemyID;

        /// <summary>
        /// 敌人数量
        /// </summary>
        public int EnemyCount;

        /// <summary>
        /// 生成间隔
        /// </summary>
        public float SpawnInterval;

        /// <summary>
        /// 波次持续时间
        /// </summary>
        public float Duration;


        // TODO: 暂时都配置在这里，后续可以考虑拆分
        public float SpawnRadius;
    }
}