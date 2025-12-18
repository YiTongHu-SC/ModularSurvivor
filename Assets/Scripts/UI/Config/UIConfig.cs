using UnityEngine;

namespace UI.Config
{
    [CreateAssetMenu(fileName = "UIConfig", menuName = "Config/UIConfig", order = 0)]
    public class UIConfig : ScriptableObject
    {
        public Vector2Int DefaultScreenSize = new(1920, 1080);
    }
}