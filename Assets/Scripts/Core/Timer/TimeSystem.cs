using System;
using System.Collections.Generic;
using Core.GameInterface;
using Utils.Core;

namespace Core.Timer
{
    public class TimeSystem : ISystem
    {
        private readonly Dictionary<int, Timer> timers = new();
        private int nextTimerId;
        private RuntimeIdAllocator Allocator = new();
        public Timer CreateTimer(float duration, Action action)
        {
            nextTimerId = Allocator.Next();
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

        public void Reset()
        {
            timers.Clear();
            Allocator.Reset();
        }
    }
}