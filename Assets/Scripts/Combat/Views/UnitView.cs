using Core.Coordinates;
using Core.Units;
using UnityEngine;

namespace Combat.Views
{
    public class UnitView : MonoBehaviour
    {
        Unit _unit;

        private void Awake()
        {
            Debug.Log("UnitView Awake");
            _unit = GetComponent<Unit>();
            if (_unit != null)
            {
                _unit.OnUpdatePosition.AddListener(UpdatePosition);
            }
        }

        private void UpdatePosition(Vector2 position)
        {
            transform.position = CoordinateConverter.ToWorldPosition(position);
        }
    }
}