using UnityEngine;

namespace Core.Units
{
    public class UnitData
    {
        public int GUID { get; set; }

        /// <summary>
        /// 单位位置坐标，x-z平面
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// 单位面向方向，0度为z方向，顺时针旋转
        /// </summary>
        public float Rotation;

        public UnitData(Vector2 position, float rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        /// <summary>
        /// 移动速度
        /// </summary>
        public float MoveSpeed;

        public Vector2 MoveDirection;

        /// <summary>
        /// 是否存活
        /// </summary>
        public bool IsActive { get; set; }

        public string MovementStrategy;
    }
}