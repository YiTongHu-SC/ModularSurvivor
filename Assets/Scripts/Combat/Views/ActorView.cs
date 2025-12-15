using Combat.Actors;
using Combat.Views.UnitViewData;
using Core.Coordinates;
using Core.Units;
using UnityEngine;

namespace Combat.Views
{
    public class ActorView : MonoBehaviour
    {
        private Actor _actor;

        private void OnEnable()
        {
            if (_actor == null)
            {
                _actor = GetComponent<Actor>();
            }

            Debug.Log("UnitView AddListeners");
            _actor.OnUpdateView.AddListener(UpdateView);
        }

        private void UpdateView(Vector2 position, float rotation)
        {
            transform.position = CoordinateConverter.ToWorldPosition(position);
            // Y轴取反, 因为逻辑坐标系是x-y平面, 而Unity是x-z平面
            transform.rotation = Quaternion.Euler(0, -rotation, 0);
        }

        private void OnDisable()
        {
            Debug.Log("UnitView RemoveListeners");
            _actor.OnUpdateView.RemoveListener(UpdateView);
        }
    }
}