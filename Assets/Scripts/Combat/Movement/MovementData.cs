using UnityEngine;

namespace Combat.Movement
{
    [CreateAssetMenu(fileName = "FILENAME", menuName = "MENUNAME", order = 0)]
    public class MovementData : ScriptableObject
    {
        public float baseSpeed = 5f;
    }
}