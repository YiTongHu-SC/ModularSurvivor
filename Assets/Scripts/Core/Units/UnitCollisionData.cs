using Core.Coordinates;
using UnityEngine;

namespace Core.Units
{
    /// <summary>
    /// 单位碰撞检测数据
    /// </summary>
    [System.Serializable]
    public struct UnitCollisionData
    {
        /// <summary>
        /// 碰撞区域类型
        /// </summary>
        public CollisionAreaType AreaType;

        /// <summary>
        /// 碰撞半径（用于圆形）
        /// </summary>
        public float Radius;

        /// <summary>
        /// 碰撞尺寸（用于矩形）
        /// </summary>
        public Vector2 Size;

        /// <summary>
        /// 碰撞偏移量
        /// </summary>
        public Vector2 Offset;
    }

    public enum CollisionAreaType
    {
        Circle,
        Rectangle
    }
}