using Combat.Actors;
using Core.Units;
using UnityEngine;

namespace Combat.Data
{
    [CreateAssetMenu(fileName = "ActorViewData", menuName = "Combat Config/ActorViewData", order = 0)]
    public class ActorViewData : ScriptableObject
    {
        public float ViewHeight = 2f;
        public float CenterOffset = 1f;
        public UnitCollisionData UnitCollisionData;
    }
}