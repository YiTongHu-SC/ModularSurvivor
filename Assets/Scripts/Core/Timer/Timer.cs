using System;

namespace Core.Timer
{
    public class Timer
    {
        private readonly Action _callback;
        private readonly float _duration;
        private float _timer;

        public Timer(float duration, Action callback)
        {
            _duration = duration;
            _timer = 0f;
            _callback = callback;
            IsCompleted = false;
        }

        public int ID { get; private set; }
        public bool IsCompleted { get; private set; }


        public void SetId(int id)
        {
            ID = id;
        }

        public void Tick(float deltaTime)
        {
            _timer += deltaTime;
            if (_timer >= _duration)
            {
                _timer = _duration;
                Perform();
            }
        }

        public void Reset()
        {
            _timer = 0f;
            IsCompleted = false;
        }

        private void Perform()
        {
            _callback?.Invoke();
            IsCompleted = true;
        }
    }
}