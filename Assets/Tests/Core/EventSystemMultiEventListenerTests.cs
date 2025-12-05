using Core.Events;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Core
{
    public class EventListenerMultiple : IEventListener<GameEvents.UnitDeathEvent>,
        IEventListener<GameEvents.WaveStartEvent>
    {
        public bool UnitDeathEventReceived { get; private set; } = false;
        public bool WaveStartEventReceived { get; private set; } = false;

        public void OnEventReceived(GameEvents.UnitDeathEvent eventData)
        {
            UnitDeathEventReceived = true;
            Debug.Log($"UnitDeathEvent received for unit: {eventData.UnitId}");
        }

        public void OnEventReceived(GameEvents.WaveStartEvent eventData)
        {
            WaveStartEventReceived = true;
            Debug.Log($"WaveStartEvent received for wave: {eventData.WaveNumber}");
        }
    }

    [TestFixture]
    public class EventSystemMultiEventListenerTests
    {
        private GameObject eventManagerObject;
        private EventManager eventManager;

        [SetUp]
        public void Setup()
        {
            eventManagerObject = new GameObject("EventManager");
            eventManager = eventManagerObject.AddComponent<EventManager>();
            eventManager.Initialize();
        }

        [TearDown]
        public void TearDown()
        {
            if (eventManagerObject != null)
            {
                Object.DestroyImmediate(eventManagerObject);
            }
        }

        [Test]
        public void Test_MultipleEventListener_ShouldNotReceiveAllEvents()
        {
            // Arrange
            var multiListener = new EventListenerMultiple();
            eventManager.Subscribe<GameEvents.UnitDeathEvent>(multiListener);

            // Act
            var deathEvent = new GameEvents.UnitDeathEvent("Unit123", Vector3.zero);
            var waveEvent = new GameEvents.WaveStartEvent(1, 1);
            eventManager.PublishEvent(deathEvent);
            eventManager.PublishEvent(waveEvent);

            // Assert
            Assert.IsTrue(multiListener.UnitDeathEventReceived, "Multi-listener should have received UnitDeathEvent");
            Assert.IsFalse(multiListener.WaveStartEventReceived,
                "Multi-listener should NOT have received WaveStartEvent");
        }

        [Test]
        public void Test_MultipleEventListener_ShouldReceiveAllEvents()
        {
            // Arrange
            var multiListener = new EventListenerMultiple();
            eventManager.Subscribe<GameEvents.UnitDeathEvent>(multiListener);
            eventManager.Subscribe<GameEvents.WaveStartEvent>(multiListener);

            // Act
            var deathEvent = new GameEvents.UnitDeathEvent("Unit123", Vector3.zero);
            var waveEvent = new GameEvents.WaveStartEvent(1, 1);
            eventManager.PublishEvent(deathEvent);
            eventManager.PublishEvent(waveEvent);

            // Assert
            Assert.IsTrue(multiListener.UnitDeathEventReceived, "Multi-listener should have received UnitDeathEvent");
            Assert.IsTrue(multiListener.WaveStartEventReceived, "Multi-listener should have received WaveStartEvent");
        }
    }
}