using UI.Framework;

namespace UI.Game
{
    public struct LevelHudModelData
    {
        public float MaxBattleTime;
        public float BattleClock;
    }

    public class LevelHudModel : BaseModel<LevelHudModelData>
    {
        public override LevelHudModelData Value { get; protected set; }
    }
}
