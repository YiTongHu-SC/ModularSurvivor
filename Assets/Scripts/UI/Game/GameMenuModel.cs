using UI.Framework;

namespace UI.Game
{
    public struct GameMenuModelData
    {
    }

    public class GameMenuModel : BaseModel<GameMenuModelData>
    {
        public override GameMenuModelData Value { get; protected set; }
    }
}