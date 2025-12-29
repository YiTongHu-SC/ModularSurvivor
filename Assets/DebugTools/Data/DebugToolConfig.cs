using DebugTools.UI;
using UnityEngine;

namespace DebugTools.Data
{
    [CreateAssetMenu(fileName = "DebugToolConfig", menuName = "Debug Config/DebugToolConfig", order = 0)]
    public class DebugToolConfig : ScriptableObject
    {
        public InfoShow InfoShowPrefab;
    }
}
