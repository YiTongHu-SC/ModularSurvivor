using System.Collections.Generic;
using UnityEngine;
using Utils.Core;

namespace Core.Coordinates
{
    /// <summary>
    /// 2D路径计算工具
    /// </summary>
    public static class PathUtils2D
    {
        /// <summary>
        /// 计算从起点到终点的直线路径
        /// </summary>
        public static List<Vector2D> GetStraightPath(Vector2D start, Vector2D end, float stepSize = 1.0f)
        {
            List<Vector2D> path = new List<Vector2D>();
            
            float distance = Vector2D.Distance(start, end);
            if (distance < stepSize)
            {
                path.Add(start);
                path.Add(end);
                return path;
            }
            
            int steps = Mathf.CeilToInt(distance / stepSize);
            
            for (int i = 0; i <= steps; i++)
            {
                float t = (float)i / steps;
                path.Add(Vector2D.Lerp(start, end, t));
            }
            
            return path;
        }

        /// <summary>
        /// 计算绕过障碍物的简单路径
        /// </summary>
        public static List<Vector2D> GetAvoidancePath(Vector2D start, Vector2D end, Area2D obstacle)
        {
            // 如果直线路径不与障碍物相交，返回直线路径
            if (!IsPathBlocked(start, end, obstacle))
            {
                return GetStraightPath(start, end);
            }
            
            // 简单的绕行策略：通过障碍物的边界点
            if (obstacle is CircleArea2D circle)
            {
                return GetCircleAvoidancePath(start, end, circle);
            }
            else if (obstacle is RectArea2D rect)
            {
                return GetRectAvoidancePath(start, end, rect);
            }
            
            // 默认返回直线路径
            return GetStraightPath(start, end);
        }

        /// <summary>
        /// 检查路径是否被障碍物阻挡
        /// </summary>
        public static bool IsPathBlocked(Vector2D start, Vector2D end, Area2D obstacle)
        {
            if (obstacle.Contains(start) || obstacle.Contains(end))
                return true;
                
            // 简化检测：检查路径中点
            Vector2D midPoint = (start + end) * 0.5f;
            return obstacle.Contains(midPoint);
        }

        /// <summary>
        /// 计算绕过圆形障碍物的路径
        /// </summary>
        private static List<Vector2D> GetCircleAvoidancePath(Vector2D start, Vector2D end, CircleArea2D circle)
        {
            List<Vector2D> path = new List<Vector2D>();
            path.Add(start);
            
            Vector2D toCenter = circle.Center - start;
            
            // 计算切点
            Vector2D perpendicular = new Vector2D(-toCenter.y, toCenter.x).Normalized;
            Vector2D tangentPoint1 = circle.Center + perpendicular * (circle.Radius + 0.5f);
            Vector2D tangentPoint2 = circle.Center - perpendicular * (circle.Radius + 0.5f);
            
            // 选择更接近目标的切点
            float dist1 = Vector2D.Distance(tangentPoint1, end);
            float dist2 = Vector2D.Distance(tangentPoint2, end);
            
            Vector2D chosenTangent = dist1 < dist2 ? tangentPoint1 : tangentPoint2;
            path.Add(chosenTangent);
            path.Add(end);
            
            return path;
        }

        /// <summary>
        /// 计算绕过矩形障碍物的路径
        /// </summary>
        private static List<Vector2D> GetRectAvoidancePath(Vector2D start, Vector2D end, RectArea2D rect)
        {
            List<Vector2D> path = new List<Vector2D>();
            path.Add(start);
            
            // 计算矩形的四个角点
            Vector2D[] corners = new Vector2D[]
            {
                new Vector2D(rect.Min.x - 0.5f, rect.Min.y - 0.5f),
                new Vector2D(rect.Max.x + 0.5f, rect.Min.y - 0.5f),
                new Vector2D(rect.Max.x + 0.5f, rect.Max.y + 0.5f),
                new Vector2D(rect.Min.x - 0.5f, rect.Max.y + 0.5f)
            };
            
            // 找到距离终点最近的角点
            float minDistance = float.MaxValue;
            Vector2D bestCorner = corners[0];
            
            foreach (Vector2D corner in corners)
            {
                float distance = Vector2D.Distance(corner, end);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    bestCorner = corner;
                }
            }
            
            path.Add(bestCorner);
            path.Add(end);
            
            return path;
        }

        /// <summary>
        /// 平滑路径
        /// </summary>
        public static List<Vector2D> SmoothPath(List<Vector2D> originalPath, float smoothingFactor = 0.5f)
        {
            if (originalPath == null || originalPath.Count < 3)
                return originalPath;
                
            List<Vector2D> smoothedPath = new List<Vector2D>();
            smoothedPath.Add(originalPath[0]); // 保持起点
            
            for (int i = 1; i < originalPath.Count - 1; i++)
            {
                Vector2D prev = originalPath[i - 1];
                Vector2D current = originalPath[i];
                Vector2D next = originalPath[i + 1];
                
                Vector2D smoothed = Vector2D.Lerp(current, (prev + next) * 0.5f, smoothingFactor);
                smoothedPath.Add(smoothed);
            }
            
            smoothedPath.Add(originalPath[originalPath.Count - 1]); // 保持终点
            
            return smoothedPath;
        }

        /// <summary>
        /// 计算路径总长度
        /// </summary>
        public static float GetPathLength(List<Vector2D> path)
        {
            if (path == null || path.Count < 2)
                return 0f;
                
            float totalLength = 0f;
            for (int i = 1; i < path.Count; i++)
            {
                totalLength += Vector2D.Distance(path[i - 1], path[i]);
            }
            
            return totalLength;
        }

        /// <summary>
        /// 根据距离获取路径上的点
        /// </summary>
        public static Vector2D GetPointAtDistance(List<Vector2D> path, float distance)
        {
            if (path == null || path.Count == 0)
                return Vector2D.Zero;
                
            if (path.Count == 1)
                return path[0];
                
            float currentDistance = 0f;
            
            for (int i = 1; i < path.Count; i++)
            {
                float segmentLength = Vector2D.Distance(path[i - 1], path[i]);
                
                if (currentDistance + segmentLength >= distance)
                {
                    float t = (distance - currentDistance) / segmentLength;
                    return Vector2D.Lerp(path[i - 1], path[i], t);
                }
                
                currentDistance += segmentLength;
            }
            
            return path[path.Count - 1];
        }
    }
}
