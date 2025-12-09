using Core.Abstructs;
using UnityEngine;

namespace Combat.Systems
{
    public class CombatManager : BaseInstance<CombatManager>
    {
        public DamageSystem DamageSystem { get; set; } = new();
        public MovementSystem MovementSystem { get; set; } = new();
        public BuffSystem BuffSystem { get; set; } = new();
        public AbilitySystem AbilitySystem { get; set; } = new();
        public ViewSystem ViewSystem { get; set; } = new();

        public override void Initialize()
        {
            MovementSystem.Initialize();
            BuffSystem.Initialize();
            AbilitySystem.Initialize();
            ViewSystem.Initialize();
        }

        public void Tick(float deltaTime)
        {
            // 更新所有战斗系统
            BuffSystem.UpdateBuffs(deltaTime);
            MovementSystem.UpdateMovement(deltaTime);
            AbilitySystem.UpdateAbilities(deltaTime);
        }
    }
}