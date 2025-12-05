using System;
using UnityEngine;

namespace Utils.Core
{
    /// <summary>
    /// 2D逻辑坐标
    /// </summary>
    [Serializable]
    public struct Vector2D : IEquatable<Vector2D>
    {
        public float x;
        public float y;

        public Vector2D(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static readonly Vector2D Zero = new Vector2D(0, 0);
        public static readonly Vector2D One = new Vector2D(1, 1);
        public static readonly Vector2D Up = new Vector2D(0, 1);
        public static readonly Vector2D Down = new Vector2D(0, -1);
        public static readonly Vector2D Left = new Vector2D(-1, 0);
        public static readonly Vector2D Right = new Vector2D(1, 0);

        /// <summary>
        /// 向量的长度
        /// </summary>
        public float Magnitude => Mathf.Sqrt(x * x + y * y);

        /// <summary>
        /// 向量的平方长度
        /// </summary>
        public float SqrMagnitude => x * x + y * y;

        /// <summary>
        /// 单位向量
        /// </summary>
        public Vector2D Normalized
        {
            get
            {
                float mag = Magnitude;
                if (mag > 0.00001f)
                    return new Vector2D(x / mag, y / mag);
                return Zero;
            }
        }

        // 运算符重载
        public static Vector2D operator +(Vector2D a, Vector2D b) => new Vector2D(a.x + b.x, a.y + b.y);
        public static Vector2D operator -(Vector2D a, Vector2D b) => new Vector2D(a.x - b.x, a.y - b.y);
        public static Vector2D operator -(Vector2D a) => new Vector2D(-a.x, -a.y);
        public static Vector2D operator *(Vector2D a, float d) => new Vector2D(a.x * d, a.y * d);
        public static Vector2D operator *(float d, Vector2D a) => new Vector2D(a.x * d, a.y * d);
        public static Vector2D operator /(Vector2D a, float d) => new Vector2D(a.x / d, a.y / d);

        public static bool operator ==(Vector2D lhs, Vector2D rhs)
        {
            return (lhs - rhs).SqrMagnitude < 0.0000001f;
        }

        public static bool operator !=(Vector2D lhs, Vector2D rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// 点积
        /// </summary>
        public static float Dot(Vector2D lhs, Vector2D rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y;
        }

        /// <summary>
        /// 两点之间的距离
        /// </summary>
        public static float Distance(Vector2D a, Vector2D b)
        {
            return (a - b).Magnitude;
        }

        /// <summary>
        /// 两点之间的平方距离
        /// </summary>
        public static float SqrDistance(Vector2D a, Vector2D b)
        {
            return (a - b).SqrMagnitude;
        }

        /// <summary>
        /// 线性插值
        /// </summary>
        public static Vector2D Lerp(Vector2D a, Vector2D b, float t)
        {
            t = Mathf.Clamp01(t);
            return new Vector2D(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
        }

        // 与Unity Vector2的转换
        public static implicit operator Vector2(Vector2D v) => new Vector2(v.x, v.y);
        public static implicit operator Vector2D(Vector2 v) => new Vector2D(v.x, v.y);

        public bool Equals(Vector2D other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector2D other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }

        public override string ToString()
        {
            return $"({x:F2}, {y:F2})";
        }
    }
}
