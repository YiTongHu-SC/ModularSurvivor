using UnityEngine;

namespace Utils.Core
{
    /// <summary>
    /// 2D数学工具
    /// </summary>
    public static class MathUtils2D
    {
        /// <summary>
        /// 计算两点之间的距离
        /// </summary>
        public static float Distance(Vector2D a, Vector2D b)
        {
            return Vector2D.Distance(a, b);
        }

        /// <summary>
        /// 计算两点之间的平方距离（性能优化）
        /// </summary>
        public static float SqrDistance(Vector2D a, Vector2D b)
        {
            return Vector2D.SqrDistance(a, b);
        }

        /// <summary>
        /// 计算从点A到点B的方向向量
        /// </summary>
        public static Vector2D Direction(Vector2D from, Vector2D to)
        {
            return (to - from).Normalized;
        }

        /// <summary>
        /// 计算向量的角度（以度为单位）
        /// </summary>
        public static float Angle(Vector2D vector)
        {
            return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// 计算两个向量之间的角度（以度为单位）
        /// </summary>
        public static float AngleBetween(Vector2D from, Vector2D to)
        {
            float dot = Vector2D.Dot(from.Normalized, to.Normalized);
            dot = Mathf.Clamp(dot, -1.0f, 1.0f);
            return Mathf.Acos(dot) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// 根据角度创建方向向量
        /// </summary>
        public static Vector2D AngleToDirection(float angleInDegrees)
        {
            float angleInRadians = angleInDegrees * Mathf.Deg2Rad;
            return new Vector2D(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
        }

        /// <summary>
        /// 将点绕原点旋转指定角度
        /// </summary>
        public static Vector2D RotatePoint(Vector2D point, float angleInDegrees)
        {
            float angleInRadians = angleInDegrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(angleInRadians);
            float sin = Mathf.Sin(angleInRadians);
            
            return new Vector2D(
                point.x * cos - point.y * sin,
                point.x * sin + point.y * cos
            );
        }

        /// <summary>
        /// 将点绕指定中心点旋转指定角度
        /// </summary>
        public static Vector2D RotatePointAround(Vector2D point, Vector2D center, float angleInDegrees)
        {
            Vector2D relative = point - center;
            Vector2D rotated = RotatePoint(relative, angleInDegrees);
            return rotated + center;
        }

        /// <summary>
        /// 计算点到线段的最短距离
        /// </summary>
        public static float DistanceToLineSegment(Vector2D point, Vector2D lineStart, Vector2D lineEnd)
        {
            Vector2D line = lineEnd - lineStart;
            Vector2D pointToStart = point - lineStart;
            
            float lineLength = line.SqrMagnitude;
            if (lineLength == 0) return Vector2D.Distance(point, lineStart);
            
            float t = Mathf.Clamp01(Vector2D.Dot(pointToStart, line) / lineLength);
            Vector2D projection = lineStart + t * line;
            
            return Vector2D.Distance(point, projection);
        }

        /// <summary>
        /// 获取点在线段上的投影点
        /// </summary>
        public static Vector2D ProjectPointOnLineSegment(Vector2D point, Vector2D lineStart, Vector2D lineEnd)
        {
            Vector2D line = lineEnd - lineStart;
            Vector2D pointToStart = point - lineStart;
            
            float lineLength = line.SqrMagnitude;
            if (lineLength == 0) return lineStart;
            
            float t = Mathf.Clamp01(Vector2D.Dot(pointToStart, line) / lineLength);
            return lineStart + t * line;
        }

        /// <summary>
        /// 线性插值
        /// </summary>
        public static Vector2D Lerp(Vector2D a, Vector2D b, float t)
        {
            return Vector2D.Lerp(a, b, t);
        }

        /// <summary>
        /// 平滑插值（球面插值在2D中的近似）
        /// </summary>
        public static Vector2D Slerp(Vector2D a, Vector2D b, float t)
        {
            float dot = Vector2D.Dot(a.Normalized, b.Normalized);
            dot = Mathf.Clamp(dot, -1.0f, 1.0f);
            
            float theta = Mathf.Acos(dot) * t;
            Vector2D relativeVec = (b - a * dot).Normalized;
            
            return ((a * Mathf.Cos(theta)) + (relativeVec * Mathf.Sin(theta))) * 
                   Mathf.Lerp(a.Magnitude, b.Magnitude, t);
        }

        /// <summary>
        /// 判断点是否在三角形内
        /// </summary>
        public static bool IsPointInTriangle(Vector2D point, Vector2D a, Vector2D b, Vector2D c)
        {
            float denom = (b.y - c.y) * (a.x - c.x) + (c.x - b.x) * (a.y - c.y);
            if (Mathf.Abs(denom) < 0.0001f) return false;
            
            float alpha = ((b.y - c.y) * (point.x - c.x) + (c.x - b.x) * (point.y - c.y)) / denom;
            float beta = ((c.y - a.y) * (point.x - c.x) + (a.x - c.x) * (point.y - c.y)) / denom;
            float gamma = 1 - alpha - beta;
            
            return alpha >= 0 && beta >= 0 && gamma >= 0;
        }

        /// <summary>
        /// 将值限制在指定范围内
        /// </summary>
        public static Vector2D Clamp(Vector2D value, Vector2D min, Vector2D max)
        {
            return new Vector2D(
                Mathf.Clamp(value.x, min.x, max.x),
                Mathf.Clamp(value.y, min.y, max.y)
            );
        }
    }
}
