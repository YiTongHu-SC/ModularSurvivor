
namespace Core.GameInterface
{
    public interface IManager
    {
        public bool IsInitialized { get; }
        public void Initialize();
        public void Reset();
        public void Tick(float deltaTime);
    }
}
