using UnityEngine;

namespace GameLoop.Config
{
    [CreateAssetMenu(fileName = "GameInitConfig", menuName = "Configs/GameInitConfig", order = 0)]
    public class GameInitConfig : ScriptableObject
    {
        public string version;
        public string LoadSceneName;
    }
}