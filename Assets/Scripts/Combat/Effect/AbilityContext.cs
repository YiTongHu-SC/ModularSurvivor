using System.Collections.Generic;
using UnityEngine;

namespace Combat.Effect
{
    public sealed class AbilityContext
    {
        // runtime 唯一标识
        public int SourceId;

        // 当前“工作目标”（执行过程中可能被 Fork/Resolver 改写）
        public TargetSet Targets;

        // public ContextExtra Extra;
    }

    public struct TargetSet
    {
        public List<int> Units;

        public Vector3 Point;
        // 也可以扩展：HitResult / Collider / Area 等
    }
}