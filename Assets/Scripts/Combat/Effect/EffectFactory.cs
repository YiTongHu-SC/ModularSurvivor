using Combat.Effect.Effects;
using UnityEngine;

namespace Combat.Effect
{
    public static class EffectFactory
    {
        /// <summary>
        /// 效果工厂方法，根据effectID创建对应的效果节点实例
        /// </summary>
        /// <param name="effectSpec"></param>
        /// <returns></returns>
        public static IEffectNode CreateEffectNode(EffectSpec effectSpec)
        {
            switch (effectSpec.EffectNodeType)
            {
                case EffectNodeType.Damage:
                    return new DamageEffect(effectSpec);
            }

            Debug.LogError("EffectFactory: 未知的效果类型 " + effectSpec.EffectNodeType);
            return null;
        }
    }
}