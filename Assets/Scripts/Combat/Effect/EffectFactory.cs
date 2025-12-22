using System;
using System.Collections.Generic;
using Combat.Effect.Effects;
using UnityEngine;

namespace Combat.Effect
{
    public static class EffectFactory
    {
        private static readonly Dictionary<EffectNodeType, Func<EffectSpec, BaseEffect>> _creators
            = new()
            {
                {
                    EffectNodeType.Damage,
                    (spec) => new DamageEffect(spec)
                },
                {
                    EffectNodeType.ChaseTarget,
                    (spec) => new ChaseTargetEffect(spec)
                }
            };

        /// <summary>
        /// 效果工厂方法，根据effectID创建对应的效果节点实例
        /// </summary>
        /// <param name="effectSpec"></param>
        /// <returns></returns>
        public static IEffectNode CreateEffectNode(EffectSpec effectSpec)
        {
            if (!_creators.TryGetValue(effectSpec.EffectNodeType, out var creator))
            {
                Debug.LogError("EffectFactory: 未找到对应的效果创建器 " + effectSpec.EffectNodeType);
                return null;
            }

            return creator(effectSpec);
        }
    }
}