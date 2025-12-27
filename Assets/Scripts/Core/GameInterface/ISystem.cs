
namespace Core.GameInterface
{
    public interface ISystem
    {
        public void Reset();
        public void Tick(float deltaTime);
    }
}