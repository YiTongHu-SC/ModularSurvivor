using System;

namespace Core.Timer
{
    public class Timer
    {
        private float _timer;
        private float _duration;
        private Action _callback;
        public int ID { get; private set; }
        public bool IsCompleted { get; private set; }

        public Timer(float duration, Action callback)
        {
            _duration = duration;
            _timer = 0f;
            _callback = callback;
            IsCompleted = false;
        }


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