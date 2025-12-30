using UnityEngine;

namespace DebugTools.Config
{
    [CreateAssetMenu(fileName = "LoopWorldGroundConfig", menuName = "Debug Config/LoopWorldGroundConfig", order = 0)]
    public class LoopWorldGroundConfig : ScriptableObject
    {
        public GameObject GroundViewPrefab;
        public int GridSize = 10;
        public int CellSize = 10;
        public int Scope = 3;
    }
}