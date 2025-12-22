using Combat.Ability.Data;
using Core.Units;

namespace Combat.Ability
{
    public sealed class Ability
    {
        public int UnitId { get; private set; }
        public UnitData UnitData => UnitManager.Instance.Units[UnitId];
        public AbilityData AbilityData { get; private set; }
        public bool IsActive { get; set; } = false;

        public Ability(AbilityData abilityData, int targetUnitId)
        {
            IsActive = true;
            UnitId = targetUnitId;
            AbilityData = abilityData;
        }

        /// <summary>
        /// 应用能力效果，新增能力时调用一次
        /// </summary>
        public void ApplyAbility()
        {
        }

        /// <summary>
        /// 移除能力效果，能力结束时调用一次
        /// </summary>
        public void RemoveAbility()
        {
        }

        /// <summary>
        /// 更新能力状态，每帧调用
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        public void TickAbility(float deltaTime)
        {
        }
    }
}