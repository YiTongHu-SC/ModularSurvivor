using NUnit.Framework;
using UnityEngine;
using Core.Coordinates;

namespace Tests.Core
{
    /// <summary>
    /// CoordinateConverter 单元测试
    /// </summary>
    [TestFixture]
    public class CoordinateConverterTests
    {
        private const float OffsetY = 1.0f;
        private const float Tolerance = 0.001f;

        [SetUp]
        public void SetUp()
        {
            // 每个测试前重置转换参数
            CoordinateConverter.SetConversionParams(OffsetY);
        }

        [Test]
        public void CoordinateConverter_ToWorldPosition_ShouldConvertCorrectly()
        {
            // Arrange
            Vector2D logicPosition = new Vector2D(10, 5);

            // Act
            Vector3 worldPosition = CoordinateConverter.ToWorldPosition(logicPosition);

            // Assert
            Assert.AreEqual(10.0f, worldPosition.x, Tolerance, "World X should match logic X");
            Assert.AreEqual(OffsetY, worldPosition.y, Tolerance, "World Y should be YOffset");
            Assert.AreEqual(5.0f, worldPosition.z, Tolerance, "World Z should match logic Y");
        }

        [Test]
        public void CoordinateConverter_ToLogicPosition_ShouldConvertCorrectly()
        {
            // Arrange
            Vector3 worldPosition = new Vector3(15, 2, 8);

            // Act
            Vector2D logicPosition = CoordinateConverter.ToLogicPosition(worldPosition);

            // Assert
            Assert.AreEqual(15.0f, logicPosition.x, Tolerance, "Logic X should match world X");
            Assert.AreEqual(8.0f, logicPosition.y, Tolerance, "Logic Y should match world Z");
        }

        [Test]
        public void CoordinateConverter_RoundTripConversion_ShouldMaintainValues()
        {
            // Arrange
            Vector2D originalLogic = new Vector2D(7, 3);

            // Act
            Vector3 world = CoordinateConverter.ToWorldPosition(originalLogic);
            Vector2D backToLogic = CoordinateConverter.ToLogicPosition(world);

            // Assert
            Assert.AreEqual(originalLogic.x, backToLogic.x, Tolerance, "Round trip X should be preserved");
            Assert.AreEqual(originalLogic.y, backToLogic.y, Tolerance, "Round trip Y should be preserved");
        }

        [Test]
        public void CoordinateConverter_WithScale_ShouldScaleCorrectly()
        {
            // Arrange
            CoordinateConverter.SetConversionParams(2.0f);
            Vector2D logicPosition = new Vector2D(5, 3);

            // Act
            Vector3 worldPosition = CoordinateConverter.ToWorldPosition(logicPosition);

            // Assert
            Assert.AreEqual(5.0f, worldPosition.x, Tolerance, "X should be scaled by 2");
            Assert.AreEqual(2.0f, worldPosition.y, Tolerance, "Y should be YOffset");
            Assert.AreEqual(3.0f, worldPosition.z, Tolerance, "Z should be scaled by 2");
        }

        [Test]
        public void CoordinateConverter_DistanceConversion_ShouldCalculateCorrectly()
        {
            // Arrange
            CoordinateConverter.SetConversionParams(0.5f);
            float logicDistance = 10.0f;

            // Act
            float worldDistance = CoordinateConverter.ToWorldDistance(logicDistance);
            float backToLogic = CoordinateConverter.ToLogicDistance(worldDistance);

            // Assert
            Assert.AreEqual(logicDistance, worldDistance, Tolerance, "World distance should be scaled");
            Assert.AreEqual(logicDistance, backToLogic, Tolerance, "Round trip should preserve distance");
        }

        [Test]
        public void CoordinateConverter_DirectionConversion_ShouldMaintainDirection()
        {
            // Arrange
            Vector2D logicDirection = new Vector2D(1, 0).Normalized;

            // Act
            Vector3 worldDirection = CoordinateConverter.ToWorldDirection(logicDirection);
            Vector2D backToLogic = CoordinateConverter.ToLogicDirection(worldDirection);

            // Assert
            Assert.AreEqual(1.0f, worldDirection.x, Tolerance, "World direction X should be preserved");
            Assert.AreEqual(0.0f, worldDirection.y, Tolerance, "World direction Y should be 0");
            Assert.AreEqual(0.0f, worldDirection.z, Tolerance, "World direction Z should be 0");
            Assert.AreEqual(logicDirection.x, backToLogic.x, Tolerance, "Round trip X should be preserved");
            Assert.AreEqual(logicDirection.y, backToLogic.y, Tolerance, "Round trip Y should be preserved");
        }
    }
}