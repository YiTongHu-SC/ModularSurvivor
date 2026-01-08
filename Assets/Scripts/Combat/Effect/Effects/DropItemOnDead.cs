using Core.Units;

namespace Combat.Effect.Effects
{
    public class DropItemOnDead : BaseEffect
    {
        public DropItemOnDead(EffectSpec effectSpec) : base(effectSpec)
        {
            EffectType = EffectNodeType.DropItemOnDead;
        }

        public override void Execute()
        {
        }

        protected override void OnUnitDead()
        {
            var getSource = UnitManager.Instance.TryGetAvailableUnit(Context.SourceId, out var sourceUnit);
            if (!getSource)
            {
                // TODO: 掉落物品逻辑
                // foreach (var itemKey in Spec.ItemKeys)
                // {
                //     CombatManager.Instance.ItemSystem.DropItemAtUnit(itemKey, Context.SourceId);
                // }
            }
            OnComplete();
        }
    }
}