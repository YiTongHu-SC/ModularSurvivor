
namespace Core.GameInterface
{
    public interface ISystem
    {
        public void Tick(float deltaTime);
        public void Reset();
    }
}