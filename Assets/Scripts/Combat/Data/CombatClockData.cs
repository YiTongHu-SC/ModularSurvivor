namespace Combat.Data
{
    public class CombatClockData
    {
        public float MaxBattleTime { get; private set; }
        public float BattleClock { get; private set; }

        public CombatClockData(float maxBattleTime)
        {
            MaxBattleTime = maxBattleTime;
            BattleClock = 0f;
        }

        public void SetBattleClock(float battleClock)
        {
            MaxBattleTime = battleClock;
        }

        public void ResetClock()
        {
            BattleClock = 0f;
        }

        public void UpdateClock(float deltaTime)
        {
            BattleClock += deltaTime;
            if (BattleClock > MaxBattleTime)
            {
                BattleClock = MaxBattleTime;
            }
        }
    }
}