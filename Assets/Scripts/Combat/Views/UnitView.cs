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
            _unit.OnUpdatePosition.AddListener(UpdatePosition);
        }

        private void OnDisable()
        {
            Debug.Log("UnitView RemoveListeners");
            _unit.OnUpdatePosition.RemoveListener(UpdatePosition);
        }

        private void UpdatePosition(Vector2 position)
        {
            transform.position = CoordinateConverter.ToWorldPosition(position);
        }
    }
}