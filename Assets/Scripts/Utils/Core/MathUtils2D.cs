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
        public static float Distance(Vector2 a, Vector2 b)
        {
            return Vector2.Distance(a, b);
        }

        /// <summary>
        /// 计算从点A到点B的方向向量
        /// </summary>
        public static Vector2 Direction(Vector2 from, Vector2 to)
        {
            return (to - from).normalized;
        }

        /// <summary>
        /// 计算向量的角度（以度为单位）
        /// </summary>
        public static float Angle(Vector2 vector)
        {
            return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// 计算两个向量之间的角度（以度为单位）
        /// </summary>
        public static float AngleBetween(Vector2 from, Vector2 to)
        {
            float dot = Vector2.Dot(from.normalized, to.normalized);
            dot = Mathf.Clamp(dot, -1.0f, 1.0f);
            return Mathf.Acos(dot) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// 根据角度创建方向向量
        /// </summary>
        public static Vector2 AngleToDirection(float angleInDegrees)
        {
            float angleInRadians = angleInDegrees * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
        }

        /// <summary>
        /// 将点绕原点旋转指定角度
        /// </summary>
        public static Vector2 RotatePoint(Vector2 point, float angleInDegrees)
        {
            float angleInRadians = angleInDegrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(angleInRadians);
            float sin = Mathf.Sin(angleInRadians);

            return new Vector2(
                point.x * cos - point.y * sin,
                point.x * sin + point.y * cos
            );
        }

        /// <summary>
        /// 将点绕指定中心点旋转指定角度
        /// </summary>
        public static Vector2 RotatePointAround(Vector2 point, Vector2 center, float angleInDegrees)
        {
            Vector2 relative = point - center;
            Vector2 rotated = RotatePoint(relative, angleInDegrees);
            return rotated + center;
        }

        /// <summary>
        /// 计算点到线段的最短距离
        /// </summary>
        public static float DistanceToLineSegment(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
        {
            Vector2 line = lineEnd - lineStart;
            Vector2 pointToStart = point - lineStart;

            float lineLength = line.magnitude;
            if (lineLength == 0) return Vector2.Distance(point, lineStart);

            float t = Mathf.Clamp01(Vector2.Dot(pointToStart, line) / lineLength);
            Vector2 projection = lineStart + t * line;

            return Vector2.Distance(point, projection);
        }

        /// <summary>
        /// 获取点在线段上的投影点
        /// </summary>
        public static Vector2 ProjectPointOnLineSegment(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
        {
            Vector2 line = lineEnd - lineStart;
            Vector2 pointToStart = point - lineStart;

            float lineLength = line.magnitude;
            if (lineLength == 0) return lineStart;

            float t = Mathf.Clamp01(Vector2.Dot(pointToStart, line) / lineLength);
            return lineStart + t * line;
        }

        /// <summary>
        /// 线性插值
        /// </summary>
        public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            return Vector2.Lerp(a, b, t);
        }

        /// <summary>
        /// 平滑插值（球面插值在2D中的近似）
        /// </summary>
        public static Vector2 Slerp(Vector2 a, Vector2 b, float t)
        {
            float dot = Vector2.Dot(a.normalized, b.normalized);
            dot = Mathf.Clamp(dot, -1.0f, 1.0f);

            float theta = Mathf.Acos(dot) * t;
            Vector2 relativeVec = (b - a * dot).normalized;

            return ((a * Mathf.Cos(theta)) + (relativeVec * Mathf.Sin(theta))) *
                   Mathf.Lerp(a.magnitude, b.magnitude, t);
        }

        /// <summary>
        /// 判断点是否在三角形内
        /// </summary>
        public static bool IsPointInTriangle(Vector2 point, Vector2 a, Vector2 b, Vector2 c)
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
        public static Vector2 Clamp(Vector2 value, Vector2 min, Vector2 max)
        {
            return new Vector2(
                Mathf.Clamp(value.x, min.x, max.x),
                Mathf.Clamp(value.y, min.y, max.y)
            );
        }
    }
}