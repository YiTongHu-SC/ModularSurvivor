using System;
using Core.Coordinates;
using Core.Units;
using UnityEngine;

namespace Combat.Views
{
    public class UnitView : MonoBehaviour
    {
        private Unit _unit;

        private void OnEnable()
        {
            if (_unit == null)
            {
                _unit = GetComponent<Unit>();
            }

            Debug.Log("UnitView AddListeners");
            _unit.OnUpdateView.AddListener(UpdateView);
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
            _unit.OnUpdateView.RemoveListener(UpdateView);
        }
    }
}