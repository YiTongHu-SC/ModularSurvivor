using System.Collections.Generic;
using Core.Timer;

namespace Combat.Effect
{
    public class EffectSystem
    {
        private readonly Dictionary<int, IEffectNode> _effectNodes = new();
        private readonly List<int> _effectRemoveQueue = new();

        public void Initialize()
        {
            _effectNodes.Clear();
            _effectRemoveQueue.Clear();
        }

        public void CastEffect(IEffectNode effectNode)
        {
            TimeManager.Instance.TimeSystem.CreateTimer(effectNode.Spec.Delay, () => CastEffectCall(effectNode));
        }

        private void CastEffectCall(IEffectNode effectNode)
        {
            if (_effectNodes.TryAdd(effectNode.NodeId, effectNode))
            {
                effectNode.Execute();
            }
        }

        public void TickEffects(float deltaTime)
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
                _effectNodes[nodeId].Remove();
                _effectNodes.Remove(nodeId);
            }
        }
    }
}