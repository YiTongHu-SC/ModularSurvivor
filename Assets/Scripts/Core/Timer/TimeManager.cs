using System;
using Core.GameInterface;
using StellarCore.Singleton;

namespace Core.Timer
{
    public class TimeManager : BaseInstance<TimeManager>, IManager
    {
        public TimeSystem TimeSystem { get; private set; }

        public bool IsInitialized { get; private set; }

        public override void Initialize()
        {
            base.Initialize();
            TimeSystem = new TimeSystem();
            IsInitialized = true;
        }

        public void Reset()
        {
            TimeSystem.Reset();
            TimeSystem = null;
            IsInitialized = false;
        }

        public void Tick(float deltaTime)
        {
            TimeSystem.Tick(deltaTime);
        }
    }
}