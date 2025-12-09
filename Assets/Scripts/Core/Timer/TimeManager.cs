using Core.Abstructs;

namespace Core.Timer
{
    public class TimeManager : BaseInstance<TimeManager>
    {
        public TimeSystem TimeSystem { get; private set; }

        public override void Initialize()
        {
            base.Initialize();
            TimeSystem = new TimeSystem();
        }

        public void Tick(float deltaTime)
        {
            TimeSystem.Tick(deltaTime);
        }
    }
}