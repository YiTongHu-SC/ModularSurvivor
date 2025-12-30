using System;
using System.Collections.Generic;
using Core.Events;
using Core.GameInterface;
using Core.Timer;
using Core.Units;

namespace Combat.Effect
{
    public class EffectSystem : ISystem
    {
        private readonly Dictionary<int, IEffectNode> _effectNodes = new();
        public readonly Dictionary<int, List<IEffectNode>> UnitEffectNodes = new();
        private readonly List<int> _effectRemoveQueue = new();

        public void Reset()
        {
            _effectNodes.Clear();
            _effectRemoveQueue.Clear();
            UnitEffectNodes.Clear();
        }

        public void CastEffect(IEffectNode effectNode)
        {
            effectNode.TryCast(() => { CastEffectCall(effectNode); });
        }

        private void CastEffectCall(IEffectNode effectNode)
        {
            _effectNodes.TryAdd(effectNode.NodeId, effectNode);
            if (!UnitEffectNodes.ContainsKey(effectNode.Context.SourceId))
            {
                UnitEffectNodes[effectNode.Context.SourceId] = new List<IEffectNode>();
            }
            UnitEffectNodes[effectNode.Context.SourceId].Add(effectNode);
        }

        /// <summary>
        /// 每帧更新效果系统逻辑
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Tick(float deltaTime)
        {
            _effectRemoveQueue.Clear();
            // 每帧更新效果系统逻辑
            foreach (var effectNode in _effectNodes.Values)
            {
                if (effectNode.IsComplete)
                {
                    _effectRemoveQueue.Add(effectNode.NodeId);
                }
                else
                {
                    effectNode.Tick(deltaTime);
                }
            }

            // 移除已完成的效果节点
            foreach (var nodeId in _effectRemoveQueue)
            {
                if (UnitEffectNodes.ContainsKey(_effectNodes[nodeId].Context.SourceId))
                {
                    UnitEffectNodes[_effectNodes[nodeId].Context.SourceId].Remove(_effectNodes[nodeId]);
                }
                _effectNodes[nodeId].Remove();
                _effectNodes.Remove(nodeId);
            }
        }
    }
}