using UnityEngine;

namespace Core.Coordinates
{
    /// <summary>
    ///     坐标转换工具
    /// </summary>
    public static class CoordinateConverter
    {
        /// <summary>
        ///     Y轴偏移（用于地面高度等）
        /// </summary>
        public static float YOffset { get; set; }

        /// <summary>
        ///     将2D逻辑坐标转换为3D世界坐标
        /// </summary>
        /// <param name="position2D">2D逻辑坐标</param>
        /// <param name="offset">额外偏移</param>
        /// <returns>3D世界坐标</returns>
        public static Vector3 ToWorldPosition(Vector2 position2D, Vector3 offset = default)
        {
            return new Vector3(position2D.x, YOffset, position2D.y) + offset;
        }

        /// <summary>
        ///     将3D世界坐标转换为2D逻辑坐标
        /// </summary>
        /// <param name="worldPosition">3D世界坐标</param>
        /// <returns>2D逻辑坐标</returns>
        public static Vector2 ToLogicPosition(Vector3 worldPosition)
        {
            return new Vector2(worldPosition.x, worldPosition.z);
        }

        /// <summary>
        ///     将2D逻辑距离转换为3D世界距离
        /// </summary>
        /// <param name="distance2D">2D逻辑距离</param>
        /// <returns>3D世界距离</returns>
        public static float ToWorldDistance(float distance2D)
        {
            return distance2D;
        }

        /// <summary>
        ///     将3D世界距离转换为2D逻辑距离
        /// </summary>
        /// <param name="worldDistance">3D世界距离</param>
        /// <returns>2D逻辑距离</returns>
        public static float ToLogicDistance(float worldDistance)
        {
            return worldDistance;
        }

        /// <summary>
        ///     将2D逻辑方向转换为3D世界方向
        /// </summary>
        /// <param name="direction2D">2D逻辑方向</param>
        /// <returns>3D世界方向</returns>
        public static Vector3 ToWorldDirection(Vector2 direction2D)
        {
            return new Vector3(direction2D.x, 0, direction2D.y).normalized;
        }

        /// <summary>
        ///     将3D世界方向转换为2D逻辑方向
        /// </summary>
        /// <param name="worldDirection">3D世界方向</param>
        /// <returns>2D逻辑方向</returns>
        public static Vector2 ToLogicDirection(Vector3 worldDirection)
        {
            return new Vector2(worldDirection.x, worldDirection.z).normalized;
        }

        /// <summary>
        ///     设置转换参数
        /// </summary>
        /// <param name="yOffset">Y轴偏移</param>
        public static void SetConversionParams(float yOffset = 0.0f)
        {
            YOffset = yOffset;
        }
    }
}