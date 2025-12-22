using System.Collections.Generic;
using UnityEngine;

namespace Combat.Ability
{
    public sealed class AbilityContext
    {
        // runtime 唯一标识
        public int SourceId;

        // 当前“工作目标”（执行过程中可能被 Fork/Resolver 改写）
        public TargetSet Targets;

        public object[] Extra;

        public AbilityContext(int sourceId, TargetSet targets = default, object[] extra = null)
        {
            SourceId = sourceId;
            Targets = targets;
            Extra = extra;
        }
    }

    public class TargetSet
    {
        public readonly List<int> TaregetUnits = new();

        public Vector3 Point = Vector3.zero;
        // 也可以扩展：HitResult / Collider / Area 等
    }
}