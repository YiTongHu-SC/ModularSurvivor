using System.Collections.Generic;
using Core.Events;
using Core.Units;
using DebugTools.Config;
using Lean.Pool;
using UnityEngine;

namespace DebugTools.Core
{
    public class LoopWorldGround : MonoBehaviour
    {
        public LoopWorldGroundConfig Config;
        private List<GroundData> _groundViews;
        private bool IsInitialized { get; set; } = false;
        private int TargetActorId { get; set; } = -1;
        private void Awake()
        {
            _groundViews = new List<GroundData>();
        }

        private void OnEnable()
        {
            EventManager.Instance.Subscribe<GameEvents.HeroCreated>(OnPlayerCreated);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe<GameEvents.HeroCreated>(OnPlayerCreated);
        }

        private void OnPlayerCreated(GameEvents.HeroCreated created)
        {
            IsInitialized = true;
            TargetActorId = created.HeroId;
        }

        private void Start()
        {
            UpdateGroundView();
        }

        private void FixedUpdate()
        {
            UpdateGroundView();
        }

        private void UpdateGroundView()
        {
            if (!IsInitialized) return;

            if (!UnitManager.Instance.TryGetAvailableUnit(TargetActorId, out var actor))
            {
                return;
            }

            foreach (var ground in _groundViews)
            {
                ground.IsChecked = false;
            }

            var actorGridIndex = GetGridIndex(actor.Position);
            for (int x = -Config.Scope; x <= Config.Scope + 1; x++)
            {
                for (int z = -Config.Scope; z <= Config.Scope + 1; z++)
                {
                    var cellIndex = new Vector2Int(actorGridIndex.x + x, actorGridIndex.y + z);
                    var cellPosition = GetGridPos(cellIndex);

                    // Check if the ground view for this cell already exists
                    if (_groundViews.Find(g => g.GridIndex == cellIndex) is GroundData existingGround)
                    {
                        existingGround.IsChecked = true;
                        continue;
                    }
                    else
                    {
                        GameObject groundView = LeanPool.Spawn(Config.GroundViewPrefab, cellPosition, Quaternion.identity, transform);
                        _groundViews.Add(new GroundData
                        {
                            IsChecked = true,
                            GridIndex = cellIndex,
                            GroundView = groundView
                        });
                    }
                }
            }

            // Remove unchecked ground views
            for (int i = _groundViews.Count - 1; i >= 0; i--)
            {
                if (!_groundViews[i].IsChecked)
                {
                    LeanPool.Despawn(_groundViews[i].GroundView);
                    _groundViews.RemoveAt(i);
                }
            }
        }

        private Vector3 GetGridPos(Vector2Int gridPos)
        {
            return new Vector3(gridPos.x * Config.GridSize, 0, gridPos.y * Config.GridSize);
        }

        private Vector2Int GetGridIndex(Vector2 position)
        {
            int indexX = Mathf.FloorToInt(position.x / Config.GridSize);
            int indexY = Mathf.FloorToInt(position.y / Config.GridSize);
            return new Vector2Int(indexX, indexY);
        }
    }

    public class GroundData
    {
        public bool IsChecked;
        public Vector2Int GridIndex;
        public GameObject GroundView;
    }
}