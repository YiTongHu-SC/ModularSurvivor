using Combat.Actors;
using UnityEngine;

namespace Combat.Data
{
    [CreateAssetMenu(fileName = "ActorData", menuName = "Combat Config/ActorData", order = 0)]
    public class ActorData : ScriptableObject
    {
        public int ActorId;
        public string ActorName;
        public Actor ActorPrefab;
    }
}