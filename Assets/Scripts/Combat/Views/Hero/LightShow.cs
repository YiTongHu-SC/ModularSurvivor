using Combat.Actors;
using UnityEngine;

namespace Combat.Views.Hero
{
    public class LightShow : MonoBehaviour
    {
        public Light LightSource;
        private Actor _actor;
        private float SightRange => _actor.UnitData.SightRange;

        private void Awake()
        {
            _actor = GetComponent<Actor>();
        }

        private void Start()
        {
            InitializeLight();
        }

        private void Update()
        {
            UpdateLightScope(SightRange);
        }

        private void InitializeLight()
        {
            if (LightSource != null)
            {
                LightSource.type = LightType.Spot;
                LightSource.color = Color.white;
                LightSource.innerSpotAngle = 35f;
                LightSource.spotAngle = 120f;
                LightSource.range = 10f;
                LightSource.intensity = 80f;
            }
        }

        private void UpdateLightScope(float scope)
        {
            if (LightSource != null)
            {
                LightSource.range = scope;
                LightSource.transform.localPosition = new Vector3(0, 0.5f * scope, 0);
            }
        }
    }
}