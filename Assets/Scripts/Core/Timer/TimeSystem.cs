using System;
using System.Collections.Generic;

namespace Core.Timer
{
    public class TimeSystem
    {
        private readonly Dictionary<int, Timer> timers = new();
        private int nextTimerId = -1;

        public Timer CreateTimer(float duration, Action action)
        {
            nextTimerId++;
            var timer = new Timer(duration, action);
            timer.SetId(nextTimerId);
            timers.Add(timer.ID, timer);
            return timer;
        }

        public void Tick(float deltaTime)
        {
            // 在这里实现计时器系统的更新逻辑
            var timersToRemove = new List<int>();
            foreach (var timer in timers.Values)
            {
                timer.Tick(deltaTime);
                if (timer.IsCompleted) timersToRemove.Add(timer.ID);
            }

            foreach (var timerId in timersToRemove) timers.Remove(timerId);
        }
    }
}