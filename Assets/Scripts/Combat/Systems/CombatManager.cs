using Core.Abstructs;
using UnityEngine;

namespace Combat.Systems
{
    public class CombatManager : BaseInstance<CombatManager>
    {
        public MovementSystem MovementSystem { get; set; } = new();
        public BuffSystem BuffSystem { get; set; } = new();
        // public DamageSystem DamageSystem { get; set; } = new();

        public override void Initialize()
        {
            MovementSystem.Initialize();
            BuffSystem.Initialize();
            // DamageSystem.Initialize();
        }

        public void Tick(float deltaTime)
        {
            // 更新所有战斗系统
            BuffSystem.UpdateBuffs(deltaTime);
            MovementSystem.UpdateMovement(deltaTime);
        }
    }
}