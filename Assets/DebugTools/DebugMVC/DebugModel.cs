using System;
using UI.Framework;

namespace DebugTools.DebugMVC
{
    [Serializable]
    public struct GameDebugData
    {
    }

    public class DebugModel : SimpleModel<GameDebugData>
    {
        public DebugModel(GameDebugData initialValue = default) : base(initialValue)
        {
        }
    }
}