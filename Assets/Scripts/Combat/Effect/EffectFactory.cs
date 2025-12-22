using Combat.Effect.Effects;
using UnityEngine;

namespace Combat.Effect
{
    public static class EffectFactory
    {
        /// <summary>
        /// 效果工厂方法，根据effectID创建对应的效果节点实例
        /// </summary>
        /// <param name="effectID">GUID</param>
        /// <returns></returns>
        public static IEffectNode CreateEffectNode(int effectID)
        {
            return new DamageEffect();
        }

        public static IEffectNode CreateEffectNode(EffectSpec spec)
        {
            switch (spec.type)
            {
                case EffectNodeType.Damage:
                    return new DamageEffect();
            }

            Debug.LogError("EffectFactory: 未知的效果类型 " + spec.type);
            return null;
        }
    }
}