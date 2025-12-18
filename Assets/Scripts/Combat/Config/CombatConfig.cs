using UnityEngine;

namespace Combat.Config
{
    [CreateAssetMenu(fileName = "CombatConfig", menuName = "CombatConfig/CombatConfig", order = 0)]
    public class CombatConfig : ScriptableObject
    {
        [Tooltip("Maximum time allowed for a battle in seconds.")]
        public float MaxBattleTime = 300f;
    }
}