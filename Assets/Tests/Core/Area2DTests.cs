using NUnit.Framework;
using Core.Coordinates;
using Utils.Core;

namespace Tests.Core
{
    /// <summary>
    /// Area2D 相关类的单元测试
    /// </summary>
    [TestFixture]
    public class Area2DTests
    {
        #region Bounds2D Tests

        [Test]
        public void Bounds2D_Construction_ShouldSetCorrectValues()
        {
            // Arrange
            Vector2D min = new Vector2D(1, 2);
            Vector2D max = new Vector2D(5, 8);
            
            // Act
            Bounds2D bounds = new Bounds2D(min, max);
            
            // Assert
            Assert.AreEqual(min, bounds.Min);
            Assert.AreEqual(max, bounds.Max);
        }

        [Test]
        public void Bounds2D_Center_ShouldCalculateCorrectly()
        {
            // Arrange
            Bounds2D bounds = new Bounds2D(new Vector2D(0, 0), new Vector2D(4, 6));
            
            // Act
            Vector2D center = bounds.Center;
            
            // Assert
            Assert.AreEqual(new Vector2D(2, 3), center);
        }

        [Test]
        public void Bounds2D_Size_ShouldCalculateCorrectly()
        {
            // Arrange
            Bounds2D bounds = new Bounds2D(new Vector2D(1, 2), new Vector2D(5, 8));
            
            // Act
            Vector2D size = bounds.Size;
            
            // Assert
            Assert.AreEqual(new Vector2D(4, 6), size);
        }

        [Test]
        public void Bounds2D_Contains_ShouldDetectCorrectly()
        {
            // Arrange
            Bounds2D bounds = new Bounds2D(new Vector2D(0, 0), new Vector2D(10, 10));
            
            // Act & Assert
            Assert.IsTrue(bounds.Contains(new Vector2D(5, 5)), "Point inside should be contained");
            Assert.IsTrue(bounds.Contains(new Vector2D(0, 0)), "Min point should be contained");
            Assert.IsTrue(bounds.Contains(new Vector2D(10, 10)), "Max point should be contained");
            Assert.IsFalse(bounds.Contains(new Vector2D(-1, 5)), "Point outside left should not be contained");
            Assert.IsFalse(bounds.Contains(new Vector2D(11, 5)), "Point outside right should not be contained");
        }

        [Test]
        public void Bounds2D_Intersects_ShouldDetectCorrectly()
        {
            // Arrange
            Bounds2D bounds1 = new Bounds2D(new Vector2D(0, 0), new Vector2D(5, 5));
            Bounds2D bounds2 = new Bounds2D(new Vector2D(3, 3), new Vector2D(8, 8));
            Bounds2D bounds3 = new Bounds2D(new Vector2D(6, 6), new Vector2D(10, 10));
            
            // Act & Assert
            Assert.IsTrue(bounds1.Intersects(bounds2), "Overlapping bounds should intersect");
            Assert.IsFalse(bounds1.Intersects(bounds3), "Non-overlapping bounds should not intersect");
        }

        #endregion

        #region CircleArea2D Tests

        [Test]
        public void CircleArea2D_Construction_ShouldSetCorrectValues()
        {
            // Arrange
            Vector2D center = new Vector2D(3, 4);
            float radius = 5.0f;
            
            // Act
            CircleArea2D circle = new CircleArea2D(center, radius);
            
            // Assert
            Assert.AreEqual(center, circle.Center);
            Assert.AreEqual(radius, circle.Radius);
        }

        [Test]
        public void CircleArea2D_Contains_ShouldDetectCorrectly()
        {
            // Arrange
            CircleArea2D circle = new CircleArea2D(Vector2D.Zero, 5.0f);
            
            // Act & Assert
            Assert.IsTrue(circle.Contains(new Vector2D(0, 0)), "Center should be contained");
            Assert.IsTrue(circle.Contains(new Vector2D(3, 4)), "Point at distance 5 should be contained");
            Assert.IsTrue(circle.Contains(new Vector2D(5, 0)), "Point on edge should be contained");
            Assert.IsFalse(circle.Contains(new Vector2D(6, 0)), "Point outside radius should not be contained");
        }

        [Test]
        public void CircleArea2D_GetBounds_ShouldCalculateCorrectly()
        {
            // Arrange
            CircleArea2D circle = new CircleArea2D(new Vector2D(2, 3), 4.0f);
            
            // Act
            Bounds2D bounds = circle.GetBounds();
            
            // Assert
            Assert.AreEqual(new Vector2D(-2, -1), bounds.Min, "Min bounds should be center - radius");
            Assert.AreEqual(new Vector2D(6, 7), bounds.Max, "Max bounds should be center + radius");
        }

        [Test]
        public void CircleArea2D_Intersects_WithOtherCircle_ShouldDetectCorrectly()
        {
            // Arrange
            CircleArea2D circle1 = new CircleArea2D(new Vector2D(0, 0), 3.0f);
            CircleArea2D circle2 = new CircleArea2D(new Vector2D(4, 0), 2.0f);
            CircleArea2D circle3 = new CircleArea2D(new Vector2D(10, 0), 1.0f);
            
            // Act & Assert
            Assert.IsTrue(circle1.Intersects(circle2), "Overlapping circles should intersect");
            Assert.IsFalse(circle1.Intersects(circle3), "Non-overlapping circles should not intersect");
        }

        #endregion

        #region RectArea2D Tests

        [Test]
        public void RectArea2D_Construction_ShouldSetCorrectValues()
        {
            // Arrange
            Vector2D min = new Vector2D(1, 2);
            Vector2D max = new Vector2D(5, 8);
            
            // Act
            RectArea2D rect = new RectArea2D(min, max);
            
            // Assert
            Assert.AreEqual(min, rect.Min);
            Assert.AreEqual(max, rect.Max);
        }

        [Test]
        public void RectArea2D_CreateFromCenterAndSize_ShouldCreateCorrectly()
        {
            // Arrange
            Vector2D center = new Vector2D(5, 5);
            Vector2D size = new Vector2D(4, 6);
            
            // Act
            RectArea2D rect = RectArea2D.CreateFromCenterAndSize(center, size);
            
            // Assert
            Assert.AreEqual(new Vector2D(3, 2), rect.Min, "Min should be center - halfSize");
            Assert.AreEqual(new Vector2D(7, 8), rect.Max, "Max should be center + halfSize");
        }

        [Test]
        public void RectArea2D_CenterAndSize_ShouldCalculateCorrectly()
        {
            // Arrange
            RectArea2D rect = new RectArea2D(new Vector2D(2, 3), new Vector2D(8, 9));
            
            // Act
            Vector2D center = rect.Center;
            Vector2D size = rect.Size;
            
            // Assert
            Assert.AreEqual(new Vector2D(5, 6), center);
            Assert.AreEqual(new Vector2D(6, 6), size);
        }

        [Test]
        public void RectArea2D_Contains_ShouldDetectCorrectly()
        {
            // Arrange
            RectArea2D rect = new RectArea2D(new Vector2D(0, 0), new Vector2D(10, 10));
            
            // Act & Assert
            Assert.IsTrue(rect.Contains(new Vector2D(5, 5)), "Point inside should be contained");
            Assert.IsTrue(rect.Contains(new Vector2D(0, 0)), "Min corner should be contained");
            Assert.IsTrue(rect.Contains(new Vector2D(10, 10)), "Max corner should be contained");
            Assert.IsFalse(rect.Contains(new Vector2D(-1, 5)), "Point outside should not be contained");
        }

        [Test]
        public void RectArea2D_Intersects_WithOtherRect_ShouldDetectCorrectly()
        {
            // Arrange
            RectArea2D rect1 = new RectArea2D(new Vector2D(0, 0), new Vector2D(5, 5));
            RectArea2D rect2 = new RectArea2D(new Vector2D(3, 3), new Vector2D(8, 8));
            RectArea2D rect3 = new RectArea2D(new Vector2D(6, 6), new Vector2D(10, 10));
            
            // Act & Assert
            Assert.IsTrue(rect1.Intersects(rect2), "Overlapping rects should intersect");
            Assert.IsFalse(rect1.Intersects(rect3), "Non-overlapping rects should not intersect");
        }

        [Test]
        public void RectArea2D_GetBounds_ShouldReturnSelf()
        {
            // Arrange
            Vector2D min = new Vector2D(1, 2);
            Vector2D max = new Vector2D(5, 8);
            RectArea2D rect = new RectArea2D(min, max);
            
            // Act
            Bounds2D bounds = rect.GetBounds();
            
            // Assert
            Assert.AreEqual(min, bounds.Min);
            Assert.AreEqual(max, bounds.Max);
        }

        #endregion

        #region PolygonArea2D Tests

        [Test]
        public void PolygonArea2D_Construction_ShouldSetCorrectValues()
        {
            // Arrange
            Vector2D[] vertices = new Vector2D[]
            {
                new Vector2D(0, 0),
                new Vector2D(2, 0),
                new Vector2D(1, 2)
            };
            
            // Act
            PolygonArea2D polygon = new PolygonArea2D(vertices);
            
            // Assert
            Assert.AreEqual(vertices, polygon.Vertices);
        }

        [Test]
        public void PolygonArea2D_Contains_Triangle_ShouldDetectCorrectly()
        {
            // Arrange
            Vector2D[] triangleVertices = new Vector2D[]
            {
                new Vector2D(0, 0),
                new Vector2D(4, 0),
                new Vector2D(2, 4)
            };
            PolygonArea2D triangle = new PolygonArea2D(triangleVertices);
            
            // Act & Assert
            Assert.IsTrue(triangle.Contains(new Vector2D(2, 1)), "Point inside triangle should be contained");
            Assert.IsFalse(triangle.Contains(new Vector2D(0, 2)), "Point outside triangle should not be contained");
            Assert.IsTrue(triangle.Contains(new Vector2D(1, 0.5f)), "Point inside triangle should be contained");
        }

        [Test]
        public void PolygonArea2D_GetBounds_ShouldCalculateCorrectly()
        {
            // Arrange
            Vector2D[] vertices = new Vector2D[]
            {
                new Vector2D(1, 2),
                new Vector2D(5, 1),
                new Vector2D(3, 6),
                new Vector2D(0, 4)
            };
            PolygonArea2D polygon = new PolygonArea2D(vertices);
            
            // Act
            Bounds2D bounds = polygon.GetBounds();
            
            // Assert
            Assert.AreEqual(new Vector2D(0, 1), bounds.Min, "Min should be minimum X and Y from vertices");
            Assert.AreEqual(new Vector2D(5, 6), bounds.Max, "Max should be maximum X and Y from vertices");
        }

        [Test]
        public void PolygonArea2D_EmptyVertices_ShouldHandleGracefully()
        {
            // Arrange
            PolygonArea2D emptyPolygon = new PolygonArea2D(null);
            PolygonArea2D twoPointPolygon = new PolygonArea2D(new[]
            {
                new Vector2D(0, 0),
                new Vector2D(1, 1)
            });
            
            // Act & Assert
            Assert.IsFalse(emptyPolygon.Contains(Vector2D.Zero), "Empty polygon should not contain any point");
            Assert.IsFalse(twoPointPolygon.Contains(Vector2D.Zero), "Polygon with < 3 vertices should not contain any point");
        }

        #endregion

        #region Mixed Area Intersections

        [Test]
        public void Area2D_CircleRectIntersection_ShouldDetectCorrectly()
        {
            // Arrange
            CircleArea2D circle = new CircleArea2D(new Vector2D(2, 2), 2.0f);
            RectArea2D rect = new RectArea2D(new Vector2D(1, 1), new Vector2D(3, 3));
            RectArea2D farRect = new RectArea2D(new Vector2D(10, 10), new Vector2D(15, 15));
            
            // Act & Assert
            Assert.IsTrue(circle.Intersects(rect), "Circle should intersect overlapping rect");
            Assert.IsTrue(rect.Intersects(circle), "Rect should intersect overlapping circle");
            Assert.IsFalse(circle.Intersects(farRect), "Circle should not intersect far rect");
            Assert.IsFalse(farRect.Intersects(circle), "Far rect should not intersect circle");
        }

        #endregion
    }
}
