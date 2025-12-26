using System;

namespace Utils.Core
{
    public sealed class RuntimeIdAllocator
    {
        private int _nextId;

        public int Next()
        {
            return ++_nextId;
        }

        public void Initialize()
        {
            _nextId = 0;
        }

        public void Reset()
        {
            _nextId = 0;
        }
    }
}