namespace Utils.Core
{
    public sealed class RuntimeIdAllocator
    {
        private int _nextId = -1;

        private int Next()
        {
            return ++_nextId;
        }

        public void Initialize()
        {
            _nextId = -1;
        }
    }
}