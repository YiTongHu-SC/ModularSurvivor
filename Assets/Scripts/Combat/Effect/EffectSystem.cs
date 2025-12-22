using System.Collections.Generic;

namespace Combat.Effect
{
    public class EffectSystem
    {
        private readonly Dictionary<int, IEffectNode> _effectNodes = new();
        private readonly List<int> _effectRemoveQueue = new();

        public void ApplyEffect(int nodeId, IEffectNode effectNode)
        {
            if (_effectNodes.TryAdd(nodeId, effectNode))
            {
                effectNode.Execute();
            }
        }

        public void Tick(float deltaTime)
        {
            // 每帧更新效果系统逻辑
            foreach (var effectNode in _effectNodes.Values)
            {
                // 这里可以传入适当的上下文
                effectNode.Tick(deltaTime);
                if (effectNode.IsComplete)
                {
                    _effectRemoveQueue.Add(effectNode.GetHashCode());
                }
            }

            // 移除已完成的效果节点
            foreach (var nodeId in _effectRemoveQueue)
            {
                _effectNodes.Remove(nodeId);
            }

            _effectRemoveQueue.Clear();
        }
    }
}