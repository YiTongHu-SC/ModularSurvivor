using UnityEngine;

namespace Core.Units
{
    public class UnitData
    {
        /// <summary>
        /// 单位位置坐标
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// 单位面向方向，0度为正上方，顺时针旋转
        /// </summary>
        public float Rotation;

        public UnitData(Vector2 position, float rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}