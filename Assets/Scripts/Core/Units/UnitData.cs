using System;
using Core.Coordinates;
using UnityEngine;
using Utils.Core;

namespace Core.Units
{
    public enum GroupType
    {
        Hero,
        Enemy
    }

    public class UnitData
    {
        /// <summary>
        /// Runtime 唯一标识符
        /// </summary>
        public int RuntimeId { get; set; } = -1;
        public string Key { get; set; } = "";
        //碰撞检测数据
        public UnitCollisionData CollisionData;
        public GroupType Group = GroupType.Hero;

        /// <summary>
        ///     模型数据
        /// </summary>
        public UnitModelView ModelView;
        public Vector2 MoveDirection; // 移动方向单位向量
        public float SightRange = 20f; // 视野范围

        public MovementContext MovementContext = new();

        public string MovementStrategy = "";

        /// <summary>
        ///     移动速度
        /// </summary>
        public float MoveSpeed;

        /// <summary>
        ///     单位位置坐标，x-z平面
        /// </summary>
        public Vector2 Position;

        /// <summary>
        ///     单位面向方向，0度为z方向，顺时针旋转
        /// </summary>
        public float Rotation;

        public UnitData(Vector2 position = default, float rotation = 0, UnitCollisionData collisionData = default,
            UnitModelView modelView = default)
        {
            Position = position;
            Rotation = rotation;
            CollisionData = collisionData;
            ModelView = modelView;
        }

        public float MaxHealth { get; set; }

        /// <summary>
        ///     是否存活
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        ///     当前血量
        /// </summary>
        public float Health { get; set; }

        /// <summary>
        ///     获取单位的碰撞区域
        /// </summary>
        public Area2D GetCollisionArea()
        {
            var actualPosition = Position + CollisionData.Offset;

            return CollisionData.AreaType switch
            {
                CollisionAreaType.Circle => new CircleArea2D(
                    new Vector2D(actualPosition.x, actualPosition.y),
                    CollisionData.Radius),
                CollisionAreaType.Rectangle => new RectArea2D(
                    new Vector2D(actualPosition.x - CollisionData.Size.x * 0.5f,
                        actualPosition.y - CollisionData.Size.y * 0.5f),
                    new Vector2D(actualPosition.x + CollisionData.Size.x * 0.5f,
                        actualPosition.y + CollisionData.Size.y * 0.5f)),
                _ => throw new ArgumentException("Unsupported collision area type")
            };
        }

        public void SetHealth(float value)
        {
            MaxHealth = value;
            Health = MaxHealth;
        }

        public MovementContext GetMovementContext()
        {
            return MovementContext;
        }

        public void SetHealth(float currentHealth, float maxHealth)
        {
            MaxHealth = maxHealth;
            Health = currentHealth;
        }
    }

    public struct MovementContext
    {
        public Vector2 targetPosition;
    }
}