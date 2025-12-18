using Combat.Actors;
using Combat.Data;
using StellarCore.Singleton;

namespace Combat.Systems
{
    public class CombatManager : BaseInstance<CombatManager>
    {
        public DamageSystem DamageSystem { get; set; } = new();
        public MovementSystem MovementSystem { get; set; } = new();
        public BuffSystem BuffSystem { get; set; } = new();
        public AbilitySystem AbilitySystem { get; set; } = new();
        public ViewSystem ViewSystem { get; set; } = new();
        public Actor HeroActor { get; set; }
        public ActorFactory ActorFactory { get; set; } = new();
        public CombatClockData CombatClock { get; set; }

        public override void Initialize()
        {
            AbilitySystem.Initialize();
            BuffSystem.Initialize();
            MovementSystem.Initialize();
            ViewSystem.Initialize();
            CombatClock = new CombatClockData(300f); // 默认战斗时间300秒
        }

        public void Tick(float deltaTime)
        {
            // 更新所有战斗系统
            AbilitySystem.UpdateAbilities(deltaTime);
            BuffSystem.UpdateBuffs(deltaTime);
            MovementSystem.UpdateMovement(deltaTime);
        }
    }
}