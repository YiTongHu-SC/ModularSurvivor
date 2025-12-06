using Core.Units;

namespace Combat.Buff
{
    public class BuffSpeedBoost : Buff
    {
        private UnitData TargetUnitData => UnitManager.Instance.Units[TargetUnitId];

        public BuffSpeedBoost(BuffData data, int targetUnitId) : base(data, targetUnitId)
        {
        }

        /// <summary>
        /// 应用Buff效果到单位属性,
        /// DOT/HOT类型的Buff不需要在这里处理，由UpdateEffect处理
        /// </summary>
        public override void ApplyEffect()
        {
            TargetUnitData.MoveSpeed *= GetEffectValue();
        }

        public override void RemoveEffect()
        {
            TargetUnitData.MoveSpeed /= GetEffectValue();
        }

        public override void UpdateEffect(float deltaTime)
        {
        }
    }
}