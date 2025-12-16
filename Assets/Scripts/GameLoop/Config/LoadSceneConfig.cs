using UnityEngine;

namespace GameLoop.Config
{
    [CreateAssetMenu(fileName = "LoadSceneConfig", menuName = "Game Config/LoadSceneConfig", order = 1)]
    public class LoadSceneConfig : ScriptableObject
    {
        public string Version;
        public string LoadSceneName;
    }
}