using UnityEngine;

namespace GameLoop.Config
{
    [CreateAssetMenu(fileName = "LoadSceneConfig", menuName = "Configs/LoadSceneConfig", order = 0)]
    public class LoadSceneConfig : ScriptableObject
    {
        public string Version;
        public string LoadSceneName;
    }
}