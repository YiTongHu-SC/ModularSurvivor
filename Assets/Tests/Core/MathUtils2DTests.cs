using NUnit.Framework;
using Core.Coordinates;
using Utils.Core;

namespace Tests.Core
{
    /// <summary>
    /// MathUtils2D 单元测试
    /// </summary>
    [TestFixture]
    public class MathUtils2DTests
    {
        [Test]
        public void MathUtils2D_Distance_ShouldCalculateCorrectly()
        {
            // Arrange
            Vector2D pointA = new Vector2D(0, 0);
            Vector2D pointB = new Vector2D(3, 4);
            
            // Act
            float distance = MathUtils2D.Distance(pointA, pointB);
            
            // Assert
            Assert.AreEqual(5.0f, distance, 0.001f, "Distance should be 5 for 3-4-5 triangle");
        }

        [Test]
        public void MathUtils2D_Direction_ShouldCalculateCorrectly()
        {
            // Arrange
            Vector2D from = new Vector2D(0, 0);
            Vector2D to = new Vector2D(3, 4);
            
            // Act
            Vector2D direction = MathUtils2D.Direction(from, to);
            
            // Assert
            Assert.AreEqual(0.6f, direction.x, 0.001f, "Direction X should be 0.6");
            Assert.AreEqual(0.8f, direction.y, 0.001f, "Direction Y should be 0.8");
            Assert.AreEqual(1.0f, direction.Magnitude, 0.001f, "Direction should be normalized");
        }

        [Test]
        public void MathUtils2D_RotatePoint_ShouldRotateCorrectly()
        {
            // Arrange
            Vector2D point = new Vector2D(1, 0);
            
            // Act
            Vector2D rotated90 = MathUtils2D.RotatePoint(point, 90);
            Vector2D rotated180 = MathUtils2D.RotatePoint(point, 180);
            
            // Assert
            Assert.AreEqual(0.0f, rotated90.x, 0.001f, "90° rotation X should be 0");
            Assert.AreEqual(1.0f, rotated90.y, 0.001f, "90° rotation Y should be 1");
            Assert.AreEqual(-1.0f, rotated180.x, 0.001f, "180° rotation X should be -1");
            Assert.AreEqual(0.0f, rotated180.y, 0.001f, "180° rotation Y should be 0");
        }

        [Test]
        public void MathUtils2D_RotatePointAround_ShouldRotateAroundCenter()
        {
            // Arrange
            Vector2D point = new Vector2D(2, 0);
            Vector2D center = new Vector2D(1, 0);
            
            // Act
            Vector2D rotated = MathUtils2D.RotatePointAround(point, center, 90);
            
            // Assert
            Assert.AreEqual(1.0f, rotated.x, 0.001f, "Rotated around center X should be 1");
            Assert.AreEqual(1.0f, rotated.y, 0.001f, "Rotated around center Y should be 1");
        }

        [Test]
        public void MathUtils2D_DistanceToLineSegment_ShouldCalculateCorrectly()
        {
            // Arrange
            Vector2D point = new Vector2D(0, 1);
            Vector2D lineStart = new Vector2D(-1, 0);
            Vector2D lineEnd = new Vector2D(1, 0);
            
            // Act
            float distance = MathUtils2D.DistanceToLineSegment(point, lineStart, lineEnd);
            
            // Assert
            Assert.AreEqual(1.0f, distance, 0.001f, "Distance to horizontal line should be 1");
        }

        [Test]
        public void MathUtils2D_IsPointInTriangle_ShouldDetectCorrectly()
        {
            // Arrange
            Vector2D a = new Vector2D(0, 0);
            Vector2D b = new Vector2D(2, 0);
            Vector2D c = new Vector2D(1, 2);
            
            Vector2D insidePoint = new Vector2D(1, 0.5f);
            Vector2D outsidePoint = new Vector2D(2, 2);
            Vector2D edgePoint = new Vector2D(0, 0);
            
            // Act
            bool isInside = MathUtils2D.IsPointInTriangle(insidePoint, a, b, c);
            bool isOutside = MathUtils2D.IsPointInTriangle(outsidePoint, a, b, c);
            bool isOnEdge = MathUtils2D.IsPointInTriangle(edgePoint, a, b, c);
            
            // Assert
            Assert.IsTrue(isInside, "Point inside triangle should return true");
            Assert.IsFalse(isOutside, "Point outside triangle should return false");
            Assert.IsTrue(isOnEdge, "Point on edge should return true");
        }

        [Test]
        public void MathUtils2D_Clamp_ShouldClampCorrectly()
        {
            // Arrange
            Vector2D value = new Vector2D(5, -3);
            Vector2D min = new Vector2D(0, -1);
            Vector2D max = new Vector2D(3, 1);
            
            // Act
            Vector2D clamped = MathUtils2D.Clamp(value, min, max);
            
            // Assert
            Assert.AreEqual(3.0f, clamped.x, 0.001f, "X should be clamped to max");
            Assert.AreEqual(-1.0f, clamped.y, 0.001f, "Y should be clamped to min");
        }
    }
}
