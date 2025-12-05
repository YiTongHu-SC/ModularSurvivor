using NUnit.Framework;
using Core.Coordinates;
using System.Collections.Generic;
using Utils.Core;

namespace Tests.Core
{
    /// <summary>
    /// PathUtils2D 单元测试
    /// </summary>
    [TestFixture]
    public class PathUtils2DTests
    {
        [Test]
        public void PathUtils2D_GetStraightPath_ShouldGenerateCorrectPath()
        {
            // Arrange
            Vector2D start = new Vector2D(0, 0);
            Vector2D end = new Vector2D(10, 0);
            float stepSize = 2.0f;
            
            // Act
            List<Vector2D> path = PathUtils2D.GetStraightPath(start, end, stepSize);
            
            // Assert
            Assert.IsTrue(path.Count >= 2, "Path should have at least start and end points");
            Assert.AreEqual(start, path[0], "First point should be start");
            Assert.AreEqual(end, path[path.Count - 1], "Last point should be end");
            
            // Check if points are roughly stepSize apart
            for (int i = 1; i < path.Count - 1; i++)
            {
                float distance = Vector2D.Distance(path[i - 1], path[i]);
                Assert.LessOrEqual(distance, stepSize + 0.001f, "Step distance should not exceed stepSize");
            }
        }

        [Test]
        public void PathUtils2D_GetStraightPath_ShortDistance_ShouldReturnStartAndEnd()
        {
            // Arrange
            Vector2D start = new Vector2D(0, 0);
            Vector2D end = new Vector2D(0.5f, 0);
            float stepSize = 1.0f;
            
            // Act
            List<Vector2D> path = PathUtils2D.GetStraightPath(start, end, stepSize);
            
            // Assert
            Assert.AreEqual(2, path.Count, "Short path should only have start and end points");
            Assert.AreEqual(start, path[0]);
            Assert.AreEqual(end, path[1]);
        }

        [Test]
        public void PathUtils2D_IsPathBlocked_ShouldDetectObstacles()
        {
            // Arrange
            Vector2D start = new Vector2D(0, 0);
            Vector2D end = new Vector2D(10, 0);
            CircleArea2D obstacle = new CircleArea2D(new Vector2D(5, 0), 1.0f);
            CircleArea2D farObstacle = new CircleArea2D(new Vector2D(0, 10), 1.0f);
            
            // Act
            bool isBlocked = PathUtils2D.IsPathBlocked(start, end, obstacle);
            bool isNotBlocked = PathUtils2D.IsPathBlocked(start, end, farObstacle);
            
            // Assert
            Assert.IsTrue(isBlocked, "Path through obstacle should be blocked");
            Assert.IsFalse(isNotBlocked, "Path away from obstacle should not be blocked");
        }

        [Test]
        public void PathUtils2D_GetAvoidancePath_NoObstacle_ShouldReturnStraightPath()
        {
            // Arrange
            Vector2D start = new Vector2D(0, 0);
            Vector2D end = new Vector2D(10, 0);
            CircleArea2D farObstacle = new CircleArea2D(new Vector2D(0, 10), 1.0f);
            
            // Act
            List<Vector2D> path = PathUtils2D.GetAvoidancePath(start, end, farObstacle);
            
            // Assert
            Assert.AreEqual(start, path[0], "Should start at start point");
            Assert.AreEqual(end, path[path.Count - 1], "Should end at end point");
        }

        [Test]
        public void PathUtils2D_GetAvoidancePath_WithCircleObstacle_ShouldAvoidObstacle()
        {
            // Arrange
            Vector2D start = new Vector2D(0, 0);
            Vector2D end = new Vector2D(10, 0);
            CircleArea2D obstacle = new CircleArea2D(new Vector2D(5, 0), 1.0f);
            
            // Act
            List<Vector2D> path = PathUtils2D.GetAvoidancePath(start, end, obstacle);
            
            // Assert
            Assert.GreaterOrEqual(path.Count, 3, "Avoidance path should have at least 3 points");
            Assert.AreEqual(start, path[0], "Should start at start point");
            Assert.AreEqual(end, path[path.Count - 1], "Should end at end point");
            
            // Check that the middle point(s) avoid the obstacle
            for (int i = 1; i < path.Count - 1; i++)
            {
                Assert.IsFalse(obstacle.Contains(path[i]), "Path points should avoid obstacle");
            }
        }

        [Test]
        public void PathUtils2D_GetAvoidancePath_WithRectObstacle_ShouldAvoidObstacle()
        {
            // Arrange
            Vector2D start = new Vector2D(0, 0);
            Vector2D end = new Vector2D(10, 0);
            RectArea2D obstacle = new RectArea2D(new Vector2D(4, -1), new Vector2D(6, 1));
            
            // Act
            List<Vector2D> path = PathUtils2D.GetAvoidancePath(start, end, obstacle);
            
            // Assert
            Assert.GreaterOrEqual(path.Count, 3, "Avoidance path should have at least 3 points");
            Assert.AreEqual(start, path[0], "Should start at start point");
            Assert.AreEqual(end, path[path.Count - 1], "Should end at end point");
        }

        [Test]
        public void PathUtils2D_GetPathLength_ShouldCalculateCorrectly()
        {
            // Arrange
            List<Vector2D> path = new List<Vector2D>
            {
                new Vector2D(0, 0),
                new Vector2D(3, 0),
                new Vector2D(3, 4),
                new Vector2D(0, 4)
            };
            
            // Act
            float length = PathUtils2D.GetPathLength(path);
            
            // Assert
            float expectedLength = 3 + 4 + 3; // 10
            Assert.AreEqual(expectedLength, length, 0.001f, "Path length should be sum of segment lengths");
        }

        [Test]
        public void PathUtils2D_GetPathLength_EmptyPath_ShouldReturnZero()
        {
            // Arrange
            List<Vector2D> emptyPath = new List<Vector2D>();
            List<Vector2D> singlePointPath = new List<Vector2D> { Vector2D.Zero };
            
            // Act
            float emptyLength = PathUtils2D.GetPathLength(emptyPath);
            float singleLength = PathUtils2D.GetPathLength(singlePointPath);
            
            // Assert
            Assert.AreEqual(0.0f, emptyLength, "Empty path should have zero length");
            Assert.AreEqual(0.0f, singleLength, "Single point path should have zero length");
        }

        [Test]
        public void PathUtils2D_GetPointAtDistance_ShouldReturnCorrectPoint()
        {
            // Arrange
            List<Vector2D> path = new List<Vector2D>
            {
                new Vector2D(0, 0),
                new Vector2D(5, 0),
                new Vector2D(5, 5)
            };
            
            // Act
            Vector2D pointAtZero = PathUtils2D.GetPointAtDistance(path, 0);
            Vector2D pointAtHalf = PathUtils2D.GetPointAtDistance(path, 2.5f);
            Vector2D pointAtFirstSegmentEnd = PathUtils2D.GetPointAtDistance(path, 5);
            Vector2D pointAtEnd = PathUtils2D.GetPointAtDistance(path, 10);
            Vector2D pointBeyondEnd = PathUtils2D.GetPointAtDistance(path, 15);
            
            // Assert
            Assert.AreEqual(new Vector2D(0, 0), pointAtZero, "Point at distance 0 should be start");
            Assert.AreEqual(new Vector2D(2.5f, 0), pointAtHalf, "Point at distance 2.5 should be halfway on first segment");
            Assert.AreEqual(new Vector2D(5, 0), pointAtFirstSegmentEnd, "Point at distance 5 should be end of first segment");
            Assert.AreEqual(new Vector2D(5, 5), pointAtEnd, "Point at distance 10 should be end of path");
            Assert.AreEqual(new Vector2D(5, 5), pointBeyondEnd, "Point beyond path length should be end of path");
        }

        [Test]
        public void PathUtils2D_GetPointAtDistance_EmptyPath_ShouldReturnZero()
        {
            // Arrange
            List<Vector2D> emptyPath = new List<Vector2D>();
            
            // Act
            Vector2D result = PathUtils2D.GetPointAtDistance(emptyPath, 5);
            
            // Assert
            Assert.AreEqual(Vector2D.Zero, result, "Empty path should return zero vector");
        }

        [Test]
        public void PathUtils2D_GetPointAtDistance_SinglePoint_ShouldReturnThatPoint()
        {
            // Arrange
            Vector2D singlePoint = new Vector2D(3, 4);
            List<Vector2D> singlePointPath = new List<Vector2D> { singlePoint };
            
            // Act
            Vector2D result = PathUtils2D.GetPointAtDistance(singlePointPath, 10);
            
            // Assert
            Assert.AreEqual(singlePoint, result, "Single point path should return that point");
        }

        [Test]
        public void PathUtils2D_SmoothPath_ShouldSmoothCorrectly()
        {
            // Arrange
            List<Vector2D> originalPath = new List<Vector2D>
            {
                new Vector2D(0, 0),
                new Vector2D(5, 5),
                new Vector2D(10, 0),
                new Vector2D(15, 5)
            };
            
            // Act
            List<Vector2D> smoothedPath = PathUtils2D.SmoothPath(originalPath, 0.3f);
            
            // Assert
            Assert.AreEqual(originalPath.Count, smoothedPath.Count, "Smoothed path should have same point count");
            Assert.AreEqual(originalPath[0], smoothedPath[0], "Start point should be preserved");
            Assert.AreEqual(originalPath[originalPath.Count - 1], smoothedPath[smoothedPath.Count - 1], "End point should be preserved");
            
            // Middle points should be different (smoothed)
            for (int i = 1; i < originalPath.Count - 1; i++)
            {
                Assert.AreNotEqual(originalPath[i], smoothedPath[i], $"Point {i} should be smoothed");
            }
        }

        [Test]
        public void PathUtils2D_SmoothPath_ShortPath_ShouldReturnOriginal()
        {
            // Arrange
            List<Vector2D> shortPath = new List<Vector2D>
            {
                new Vector2D(0, 0),
                new Vector2D(5, 5)
            };
            
            // Act
            List<Vector2D> result = PathUtils2D.SmoothPath(shortPath, 0.3f);
            
            // Assert
            Assert.AreEqual(shortPath, result, "Short path should remain unchanged");
        }
    }
}
