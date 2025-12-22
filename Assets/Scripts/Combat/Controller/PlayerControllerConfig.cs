using UnityEngine;

namespace Combat.Controller
{
    [CreateAssetMenu(fileName = "PlayerControllerConfig", menuName = "Game Config/PlayerControllerConfig", order = 0)]
    public class PlayerControllerConfig : ScriptableObject
    {
        public float DeadZone = 0.1f;
    }
}