using System.Collections.Generic;

namespace Combat.Ability.Data
{
    public class AbilityTriggerByEventData : AbilityData
    {
        // 事件触发相关的数据可以在这里扩展
        public List<TriggerEventType> EventTypes = new();
    }
}