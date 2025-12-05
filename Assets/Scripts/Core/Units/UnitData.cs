﻿using UnityEngine;

namespace Core.Units
{
    public class UnitData
    {
        public int ID;

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

        /// <summary>
        /// 移动速度
        /// </summary>
        public float MoveSpeed;

        public Vector2 MoveDirection;
        
        /// <summary>
        /// 基础攻击力
        /// </summary>
        public float AttackPower = 10f;
        
        /// <summary>
        /// 最大生命值
        /// </summary>
        public float MaxHealth = 100f;
        
        /// <summary>
        /// 当前生命值
        /// </summary>
        public float CurrentHealth = 100f;
        
        /// <summary>
        /// 是否存活
        /// </summary>
        public bool IsAlive => CurrentHealth > 0;
    }
}