using Combat.Actors;
using Core.Units;
using UnityEngine;

namespace GameLoop.GameDebug
{
    public class CreateHeroOnStart : MonoBehaviour
    {
        public Actor HeroPrefab;
        public Vector3 SpawnPosition = Vector3.zero;

        public void CreateHero()
        {
            if (HeroPrefab != null)
            {
                var heroData = new UnitData(SpawnPosition, 0);
                UnitManager.Instance.Factory.Spawn(HeroPrefab, heroData);
            }
            else
            {
                Debug.LogError("HeroPrefab is not assigned in CreateHeroOnStart.");
            }
        }
    }
}