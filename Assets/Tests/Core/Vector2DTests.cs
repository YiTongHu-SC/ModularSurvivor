using NUnit.Framework;
using UnityEngine;
using Core.Coordinates;

namespace Tests.Core
{
    /// <summary>
    /// Vector2D 单元测试
    /// </summary>
    [TestFixture]
    public class Vector2DTests
    {
        [Test]
        public void Vector2D_Construction_ShouldSetCorrectValues()
        {
            // Arrange & Act
            Vector2D vector = new Vector2D(3, 4);
            
            // Assert
            Assert.AreEqual(3, vector.x, "X component should be 3");
            Assert.AreEqual(4, vector.y, "Y component should be 4");
        }

        [Test]
        public void Vector2D_Magnitude_ShouldCalculateCorrectly()
        {
            // Arrange
            Vector2D vector = new Vector2D(3, 4);
            
            // Act
            float magnitude = vector.Magnitude;
            
            // Assert
            Assert.AreEqual(5.0f, magnitude, 0.001f, "Magnitude should be 5");
        }

        [Test]
        public void Vector2D_SqrMagnitude_ShouldCalculateCorrectly()
        {
            // Arrange
            Vector2D vector = new Vector2D(3, 4);
            
            // Act
            float sqrMagnitude = vector.SqrMagnitude;
            
            // Assert
            Assert.AreEqual(25.0f, sqrMagnitude, 0.001f, "Square magnitude should be 25");
        }

        [Test]
        public void Vector2D_Normalized_ShouldReturnUnitVector()
        {
            // Arrange
            Vector2D vector = new Vector2D(3, 4);
            
            // Act
            Vector2D normalized = vector.Normalized;
            
            // Assert
            Assert.AreEqual(1.0f, normalized.Magnitude, 0.001f, "Normalized vector magnitude should be 1");
            Assert.AreEqual(0.6f, normalized.x, 0.001f, "Normalized X should be 0.6");
            Assert.AreEqual(0.8f, normalized.y, 0.001f, "Normalized Y should be 0.8");
        }

        [Test]
        public void Vector2D_StaticConstants_ShouldHaveCorrectValues()
        {
            // Assert
            Assert.AreEqual(new Vector2D(0, 0), Vector2D.Zero);
            Assert.AreEqual(new Vector2D(1, 1), Vector2D.One);
            Assert.AreEqual(new Vector2D(0, 1), Vector2D.Up);
            Assert.AreEqual(new Vector2D(0, -1), Vector2D.Down);
            Assert.AreEqual(new Vector2D(-1, 0), Vector2D.Left);
            Assert.AreEqual(new Vector2D(1, 0), Vector2D.Right);
        }

        [Test]
        public void Vector2D_Addition_ShouldCalculateCorrectly()
        {
            // Arrange
            Vector2D v1 = new Vector2D(1, 2);
            Vector2D v2 = new Vector2D(3, 4);
            
            // Act
            Vector2D result = v1 + v2;
            
            // Assert
            Assert.AreEqual(new Vector2D(4, 6), result);
        }

        [Test]
        public void Vector2D_Subtraction_ShouldCalculateCorrectly()
        {
            // Arrange
            Vector2D v1 = new Vector2D(5, 7);
            Vector2D v2 = new Vector2D(2, 3);
            
            // Act
            Vector2D result = v1 - v2;
            
            // Assert
            Assert.AreEqual(new Vector2D(3, 4), result);
        }

        [Test]
        public void Vector2D_ScalarMultiplication_ShouldCalculateCorrectly()
        {
            // Arrange
            Vector2D vector = new Vector2D(2, 3);
            
            // Act
            Vector2D result = vector * 2.0f;
            
            // Assert
            Assert.AreEqual(new Vector2D(4, 6), result);
        }

        [Test]
        public void Vector2D_Distance_ShouldCalculateCorrectly()
        {
            // Arrange
            Vector2D pointA = new Vector2D(0, 0);
            Vector2D pointB = new Vector2D(3, 4);
            
            // Act
            float distance = Vector2D.Distance(pointA, pointB);
            
            // Assert
            Assert.AreEqual(5.0f, distance, 0.001f, "Distance should be 5");
        }

        [Test]
        public void Vector2D_DotProduct_ShouldCalculateCorrectly()
        {
            // Arrange
            Vector2D v1 = new Vector2D(2, 3);
            Vector2D v2 = new Vector2D(4, 5);
            
            // Act
            float dot = Vector2D.Dot(v1, v2);
            
            // Assert
            Assert.AreEqual(23.0f, dot, 0.001f, "Dot product should be 23 (2*4 + 3*5)");
        }

        [Test]
        public void Vector2D_Lerp_ShouldInterpolateCorrectly()
        {
            // Arrange
            Vector2D start = new Vector2D(0, 0);
            Vector2D end = new Vector2D(10, 10);
            
            // Act
            Vector2D mid = Vector2D.Lerp(start, end, 0.5f);
            
            // Assert
            Assert.AreEqual(new Vector2D(5, 5), mid);
        }

        [Test]
        public void Vector2D_UnityConversion_ShouldWorkCorrectly()
        {
            // Arrange
            Vector2D vector2D = new Vector2D(3, 4);
            
            // Act
            Vector2 unityVector = vector2D;
            Vector2D backToVector2D = unityVector;
            
            // Assert
            Assert.AreEqual(3.0f, unityVector.x, 0.001f);
            Assert.AreEqual(4.0f, unityVector.y, 0.001f);
            Assert.AreEqual(vector2D, backToVector2D);
        }
    }
}
