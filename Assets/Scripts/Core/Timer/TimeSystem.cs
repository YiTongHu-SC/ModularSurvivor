using System.Collections.Generic;
using System;

namespace Core.Timer
{
    public class TimeSystem
    {
        private int nextTimerId = -1;
        private Dictionary<int, Timer> timers = new Dictionary<int, Timer>();

        public Timer CreateTimer(float duration, Action action)
        {
            nextTimerId++;
            Timer timer = new Timer(duration, action);
            timer.SetId(nextTimerId);
            timers.Add(timer.ID, timer);
            return timer;
        }

        public void Tick(float deltaTime)
        {
            // 在这里实现计时器系统的更新逻辑
            List<int> timersToRemove = new List<int>();
            foreach (var timer in timers.Values)
            {
                timer.Tick(deltaTime);
                if (timer.IsCompleted)
                {
                    timersToRemove.Add(timer.ID);
                }
            }

            foreach (var timerId in timersToRemove)
            {
                timers.Remove(timerId);
            }
        }
    }
}