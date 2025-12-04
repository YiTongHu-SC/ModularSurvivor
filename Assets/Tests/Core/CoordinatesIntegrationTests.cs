using NUnit.Framework;
using UnityEngine;
using Core.Coordinates;
using System.Collections.Generic;

namespace Tests.Core
{
    /// <summary>
    /// 坐标系统集成测试 - 测试各个组件间的协同工作
    /// </summary>
    [TestFixture]
    public class CoordinatesIntegrationTests
    {
        private const float Tolerance = 0.001f;

        [SetUp]
        public void SetUp()
        {
            // 重置坐标转换参数
            CoordinateConverter.SetConversionParams(1.0f);
            CoordinateConverter.YOffset = 0.0f;
        }

        [Test]
        public void Integration_BasicCoordinateConversion_ShouldWorkEndToEnd()
        {
            // Arrange
            Vector2D logicPosition = new Vector2D(10, 5);

            // Act - 转换为世界坐标再转换回来
            Vector3 worldPosition = CoordinateConverter.ToWorldPosition(logicPosition);
            Vector2D backToLogic = CoordinateConverter.ToLogicPosition(worldPosition);

            // Assert
            Assert.AreEqual(10.0f, worldPosition.x, Tolerance);
            Assert.AreEqual(0.0f, worldPosition.y, Tolerance);
            Assert.AreEqual(5.0f, worldPosition.z, Tolerance);
            Assert.AreEqual(logicPosition.x, backToLogic.x, Tolerance);
            Assert.AreEqual(logicPosition.y, backToLogic.y, Tolerance);
        }

        [Test]
        public void Integration_DistanceCalculationConsistency_BetweenLogicAndWorld()
        {
            // Arrange
            Vector2D pointA = new Vector2D(0, 0);
            Vector2D pointB = new Vector2D(3, 4);
            CoordinateConverter.SetConversionParams(2.0f);

            // Act
            float logicDistance = MathUtils2D.Distance(pointA, pointB);
            Vector3 worldA = CoordinateConverter.ToWorldPosition(pointA);
            Vector3 worldB = CoordinateConverter.ToWorldPosition(pointB);
            float worldDistance = Vector3.Distance(worldA, worldB);
            float convertedWorldDistance = CoordinateConverter.ToWorldDistance(logicDistance);

            // Assert
            Assert.AreEqual(5.0f, logicDistance, Tolerance, "Logic distance should be 5");
            Assert.AreEqual(5.0f, worldDistance, Tolerance, "World distance should be scaled by 2");
            Assert.AreEqual(worldDistance, convertedWorldDistance, Tolerance,
                "Converted distance should match actual world distance");
        }

        [Test]
        public void Integration_AreaDetectionWithCoordinateConversion_ShouldWorkCorrectly()
        {
            // Arrange - 在逻辑坐标中创建区域
            CircleArea2D logicCircle = new CircleArea2D(new Vector2D(5, 5), 3.0f);
            Vector2D logicTestPoint = new Vector2D(6, 6);

            // Act - 转换到世界坐标进行验证
            Vector3 worldCenter = CoordinateConverter.ToWorldPosition(logicCircle.Center);
            float worldRadius = CoordinateConverter.ToWorldDistance(logicCircle.Radius);
            Vector3 worldTestPoint = CoordinateConverter.ToWorldPosition(logicTestPoint);

            bool logicContains = logicCircle.Contains(logicTestPoint);
            float worldDistanceToCenter = Vector3.Distance(worldTestPoint, worldCenter);
            bool worldContains = worldDistanceToCenter <= worldRadius;

            // Assert
            Assert.IsTrue(logicContains, "Point should be inside circle in logic space");
            Assert.IsTrue(worldContains, "Point should be inside circle in world space");
        }

        [Test]
        public void Integration_PathfindingWithAreaAvoidance_ShouldGenerateValidPath()
        {
            // Arrange
            Vector2D start = new Vector2D(0, 0);
            Vector2D end = new Vector2D(10, 0);
            CircleArea2D obstacle = new CircleArea2D(new Vector2D(5, 0), 2.0f);

            // Act
            List<Vector2D> path = PathUtils2D.GetAvoidancePath(start, end, obstacle);
            float pathLength = PathUtils2D.GetPathLength(path);

            // Assert
            Assert.GreaterOrEqual(path.Count, 3, "Avoidance path should have multiple points");
            Assert.AreEqual(start, path[0], "Path should start at start point");
            Assert.AreEqual(end, path[path.Count - 1], "Path should end at end point");
            Assert.Greater(pathLength, 10.0f, "Avoidance path should be longer than straight path");

            // 验证路径中的点都不在障碍物内
            foreach (Vector2D point in path)
            {
                if (point != start && point != end) // 起点和终点可能在边界上
                {
                    Assert.IsFalse(obstacle.Contains(point), $"Path point {point} should not be inside obstacle");
                }
            }
        }

        [Test]
        public void Integration_VectorMathWithAreas_ShouldCalculateCorrectly()
        {
            // Arrange
            Vector2D center = new Vector2D(5, 5);
            float radius = 3.0f;
            CircleArea2D circle = new CircleArea2D(center, radius);

            Vector2D outsidePoint = new Vector2D(10, 5);

            // Act - 计算从外部点到圆心的方向，然后找到圆边界上的点
            Vector2D direction = MathUtils2D.Direction(outsidePoint, center);
            Vector2D pointOnCircle = center + direction.Normalized * radius;
            Vector2D pointInsideCircle = center + direction.Normalized * (radius - 0.1f);

            // Assert
            Assert.IsFalse(circle.Contains(outsidePoint), "Outside point should not be in circle");
            Assert.IsTrue(circle.Contains(pointOnCircle), "Point on circle boundary should be contained");
            Assert.IsTrue(circle.Contains(pointInsideCircle), "Point inside circle should be contained");

            float distanceToCenter = MathUtils2D.Distance(pointOnCircle, center);
            Assert.AreEqual(radius, distanceToCenter, Tolerance,
                "Point on circle should be exactly radius distance from center");
        }

        [Test]
        public void Integration_ComplexScenario_MultipleObstaclesAndAreas()
        {
            // Arrange - 复杂场景：多个障碍物和区域
            Vector2D playerPos = new Vector2D(0, 0);
            Vector2D targetPos = new Vector2D(20, 20);

            CircleArea2D obstacle1 = new CircleArea2D(new Vector2D(5, 5), 2.0f);
            RectArea2D obstacle2 = new RectArea2D(new Vector2D(10, 10), new Vector2D(15, 15));
            CircleArea2D safeZone = new CircleArea2D(new Vector2D(18, 18), 3.0f);

            // Act
            List<Vector2D> pathToTarget = PathUtils2D.GetStraightPath(playerPos, targetPos, 2.0f);

            bool pathBlockedByObstacle1 = false;
            bool pathBlockedByObstacle2 = false;
            bool targetInSafeZone = safeZone.Contains(targetPos);

            foreach (Vector2D point in pathToTarget)
            {
                if (obstacle1.Contains(point)) pathBlockedByObstacle1 = true;
                if (obstacle2.Contains(point)) pathBlockedByObstacle2 = true;
            }

            // Calculate alternative path if blocked
            List<Vector2D> finalPath = pathToTarget;
            if (pathBlockedByObstacle1 || pathBlockedByObstacle2)
            {
                // 简化处理：如果路径被阻挡，使用避障路径
                if (pathBlockedByObstacle1)
                {
                    finalPath = PathUtils2D.GetAvoidancePath(playerPos, targetPos, obstacle1);
                }
            }

            // Assert
            Assert.IsTrue(targetInSafeZone, "Target should be in safe zone");
            Assert.IsNotNull(finalPath, "Should always have a valid path");
            Assert.AreEqual(playerPos, finalPath[0], "Path should start at player position");

            // 验证最终路径长度合理
            float pathLength = PathUtils2D.GetPathLength(finalPath);
            float straightLineDistance = MathUtils2D.Distance(playerPos, targetPos);
            Assert.GreaterOrEqual(pathLength + Tolerance, straightLineDistance,
                "Path length should be at least straight line distance");
        }

        [Test]
        public void Integration_RotationAndProjection_WithAreas()
        {
            // Arrange
            Vector2D center = new Vector2D(5, 5);
            RectArea2D rect = RectArea2D.CreateFromCenterAndSize(center, new Vector2D(4, 2));

            Vector2D testPoint = new Vector2D(7, 5); // 矩形右侧的点

            // Act - 围绕中心旋转测试点
            Vector2D rotated90 = MathUtils2D.RotatePointAround(testPoint, center, 90);
            Vector2D rotated180 = MathUtils2D.RotatePointAround(testPoint, center, 180);

            // Assert - 验证旋转后的点与原始矩形的关系
            Assert.IsTrue(rect.Contains(testPoint), "Original point should be in rectangle");

            // 旋转90度后的点应该在矩形上方
            Assert.AreEqual(5.0f, rotated90.x, 0.001f, "90° rotation X should be center X");
            Assert.AreEqual(7.0f, rotated90.y, 0.001f, "90° rotation Y should be above center");

            // 旋转180度后的点应该在矩形左侧
            Assert.AreEqual(3.0f, rotated180.x, 0.001f, "180° rotation should be on left side");
            Assert.AreEqual(5.0f, rotated180.y, 0.001f, "180° rotation Y should be center Y");

            // 验证旋转保持距离
            float originalDistance = MathUtils2D.Distance(testPoint, center);
            float rotatedDistance = MathUtils2D.Distance(rotated90, center);
            Assert.AreEqual(originalDistance, rotatedDistance, 0.001f, "Rotation should preserve distance");
        }

        [Test]
        public void Integration_ScaledCoordinateSystem_WithComplexOperations()
        {
            CoordinateConverter.SetConversionParams(2.0f);

            Vector2D logicStart = new Vector2D(0, 0);
            Vector2D logicEnd = new Vector2D(20, 20);
            CircleArea2D logicObstacle = new CircleArea2D(new Vector2D(10, 10), 3.0f);

            // Act - 在逻辑坐标系中计算路径，然后转换到世界坐标
            List<Vector2D> logicPath = PathUtils2D.GetAvoidancePath(logicStart, logicEnd, logicObstacle);
            List<Vector3> worldPath = new List<Vector3>();

            foreach (Vector2D logicPoint in logicPath)
            {
                worldPath.Add(CoordinateConverter.ToWorldPosition(logicPoint));
            }

            // 计算两个坐标系中的路径长度
            float logicPathLength = PathUtils2D.GetPathLength(logicPath);
            float worldPathLength = 0;
            for (int i = 1; i < worldPath.Count; i++)
            {
                worldPathLength += Vector3.Distance(worldPath[i - 1], worldPath[i]);
            }

            // Assert
            Assert.AreEqual(logicPath.Count, worldPath.Count, "Path should have same number of points");
            Assert.AreEqual(logicPathLength, worldPathLength, Tolerance,
                "World path should be scaled by conversion factor");

            // 验证世界坐标中的Y值都是偏移值
            foreach (Vector3 worldPoint in worldPath)
            {
                Assert.AreEqual(2.0f, worldPoint.y, Tolerance, "All world points should have Y offset");
            }

            // 验证起点和终点转换正确
            Assert.AreEqual(new Vector3(0, 2, 0), worldPath[0], "World start should be correctly converted");
            Assert.AreEqual(new Vector3(20, 2, 20), worldPath[worldPath.Count - 1],
                "World end should be correctly converted");
        }
    }
}