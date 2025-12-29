using System;
using UI.Framework;

namespace DebugTools.DebugMVC
{
    [Serializable]
    public struct GameDebugData
    {
        public bool IsShowActorInfo;
        public int SelectedActorId;
    }

    public class DebugModel : SimpleModel<GameDebugData>
    {
        public DebugModel(GameDebugData initialValue = default) : base(initialValue)
        {
            Value = new GameDebugData
            {
                IsShowActorInfo = false,
                SelectedActorId = -1
            };
        }
    }
}