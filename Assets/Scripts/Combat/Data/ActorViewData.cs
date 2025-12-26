using Combat.Actors;
using Core.Units;
using UnityEngine;

namespace Combat.Data
{
    [CreateAssetMenu(fileName = "ActorViewData", menuName = "Combat Config/ActorViewData", order = 0)]
    public class ActorViewData : ScriptableObject
    {
        public UnitModelView ModelView;
        public UnitCollisionData UnitCollisionData;
    }
}