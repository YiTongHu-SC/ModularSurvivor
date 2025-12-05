using Core.Abstructs;
using UnityEngine;

namespace Combat.Systems
{
    public class CombatManager : BaseInstance<CombatManager>
    {
        public MovementSystem MovementSystem { get; set; } = new();

        public override void Initialize()
        {
        }
    }
}