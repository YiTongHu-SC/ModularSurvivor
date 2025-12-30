using UnityEngine;

namespace Combat.Buff
{
    [CreateAssetMenu(fileName = "BuffDataConfig", menuName = "Combat Config/BuffDataConfig", order = 0)]
    public class BuffDataConfig : ScriptableObject
    {
        public string Name;
        public BuffType BuffType;
        public float Duration;
        public float Value;
        public bool CanStack;
    }
}