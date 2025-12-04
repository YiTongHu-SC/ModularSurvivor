using UnityEngine;

namespace Core.Coordinates
{
    /// <summary>
    /// 坐标转换工具
    /// </summary>
    public static class CoordinateConverter
    {
        /// <summary>
        /// Y轴偏移（用于地面高度等）
        /// </summary>
        public static float YOffset { get; set; }

        /// <summary>
        /// 将2D逻辑坐标转换为3D世界坐标
        /// </summary>
        /// <param name="position2D">2D逻辑坐标</param>
        /// <returns>3D世界坐标</returns>
        public static Vector3 ToWorldPosition(Vector2D position2D)
        {
            return new Vector3(position2D.x, YOffset, position2D.y);
        }

        /// <summary>
        /// 将3D世界坐标转换为2D逻辑坐标
        /// </summary>
        /// <param name="worldPosition">3D世界坐标</param>
        /// <returns>2D逻辑坐标</returns>
        public static Vector2D ToLogicPosition(Vector3 worldPosition)
        {
            return new Vector2D(worldPosition.x, worldPosition.z);
        }

        /// <summary>
        /// 将2D逻辑距离转换为3D世界距离
        /// </summary>
        /// <param name="distance2D">2D逻辑距离</param>
        /// <returns>3D世界距离</returns>
        public static float ToWorldDistance(float distance2D)
        {
            return distance2D;
        }

        /// <summary>
        /// 将3D世界距离转换为2D逻辑距离
        /// </summary>
        /// <param name="worldDistance">3D世界距离</param>
        /// <returns>2D逻辑距离</returns>
        public static float ToLogicDistance(float worldDistance)
        {
            return worldDistance;
        }

        /// <summary>
        /// 将2D逻辑方向转换为3D世界方向
        /// </summary>
        /// <param name="direction2D">2D逻辑方向</param>
        /// <returns>3D世界方向</returns>
        public static Vector3 ToWorldDirection(Vector2D direction2D)
        {
            return new Vector3(direction2D.x, 0, direction2D.y).normalized;
        }

        /// <summary>
        /// 将3D世界方向转换为2D逻辑方向
        /// </summary>
        /// <param name="worldDirection">3D世界方向</param>
        /// <returns>2D逻辑方向</returns>
        public static Vector2D ToLogicDirection(Vector3 worldDirection)
        {
            return new Vector2D(worldDirection.x, worldDirection.z).Normalized;
        }

        /// <summary>
        /// 设置转换参数
        /// </summary>
        /// <param name="yOffset">Y轴偏移</param>
        public static void SetConversionParams(float yOffset = 0.0f)
        {
            YOffset = yOffset;
        }
    }
}