using UnityEngine;

namespace Combat.Views
{
    [CreateAssetMenu(fileName = "PresentationConfig", menuName = "Configs/PresentationConfig", order = 0)]
    public class PresentationConfig : ScriptableObject
    {
        public int PreferenceId;
        public GameObject PresentationPrefab;
    }
}