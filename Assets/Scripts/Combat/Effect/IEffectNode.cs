using Combat.Ability;

namespace Combat.Effect
{
    public interface IEffectNode
    {
        /// <summary>
        /// Runtime 唯一标识
        /// </summary>
        public int NodeId { get; }

        public EffectNodeType Type { get; }
        public AbilityContext Context { get; set; }

        // /// <summary>
        // /// 初始化效果节点
        // /// </summary>
        // /// <param name="effectSpec"></param>
        // public void OnInitialize(EffectSpec effectSpec);


        /// <summary>
        /// 设置效果节点的上下文
        /// </summary>
        /// <param name="context"></param>
        public void SetContext(AbilityContext context);

        /// <summary>
        /// 效果节点是否执行完成
        /// </summary>
        public bool IsComplete { get; }

        /// <summary>
        /// 执行效果节点的逻辑，创建时执行
        /// </summary>
        public void Execute();

        /// <summary>
        /// 每帧更新效果节点的逻辑
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Tick(float deltaTime);
    }
}