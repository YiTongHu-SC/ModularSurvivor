using System;
using Combat.Data;
using Combat.Systems;
using Core.Coordinates;
using Core.Events;
using Core.Units;
using Lean.Pool;
using UnityEngine;
using UnityEngine.Events;

namespace Combat.Actors
{
    public class Actor : MonoBehaviour,
        IPoolable,
        IEventListener<GameEvents.UnitDeathEvent>,
        IEventListener<GameEvents.UnitMovementEvent>
    {
        public CollisionAreaType CollisionAreaType;
        [Tooltip("视图数据")]
        public ActorViewData ViewData;
        // 这里可以添加Actor特有的属性和方法
        public bool IsActive => UnitData.IsActive;
        public UnitData UnitData { get; protected set; }
        public int RuntimeId => UnitData.RuntimeId;
        public bool Initialized { get; private set; }
        public Transform ModelTransform { get; set; }

        private UnityEvent _onInitialize;

        // 事件
        public UnityEvent OnInitialize
        {
            get
            {
                if (_onInitialize == null)
                {
                    _onInitialize = new UnityEvent();
                }

                return _onInitialize;
            }
        }

        private UnityEvent<Vector2, float> _onUpdateShow;

        public UnityEvent<Vector2, float> OnUpdateView
        {
            get
            {
                if (_onUpdateShow == null)
                {
                    _onUpdateShow = new UnityEvent<Vector2, float>();
                }

                return _onUpdateShow;
            }
        }

        private void Awake()
        {
            Debug.Log("Unit Awake");
            ModelTransform = transform.GetChild(0);
            ModelTransform.gameObject.SetActive(false);
            ModelTransform.gameObject.SetActive(true);
        }

        public virtual void Initialize(UnitData data)
        {
            Debug.Log("Unit Initialize");
            UnitData = data;
            UnitData.IsActive = true;
            // UnitData.
            UnitData.ModelView = ViewData.ModelView;
            UnitData.CollisionData = ViewData.UnitCollisionData;
            Initialized = true;
            UnitManager.Instance.UnitSystem.RegisterUnit(UnitData);
            UpdatePosView(data.Position, data.Rotation);
        }

        private void Start()
        {
            if (!Initialized) return;
            Debug.Log("Unit Start");
            OnInitialize?.Invoke();
        }

        private void OnEnable()
        {
            OnUpdateView.AddListener(UpdatePosView);
        }

        private void OnDisable()
        {
            OnUpdateView.RemoveListener(UpdatePosView);
        }

        public virtual void OnSpawn()
        {
            Debug.Log("Unit Spawned");
            Initialized = false;
            EventManager.Instance.Subscribe<GameEvents.UnitDeathEvent>(this);
            EventManager.Instance.Subscribe<GameEvents.UnitMovementEvent>(this);
        }

        public virtual void OnDespawn()
        {
            UnitData.IsActive = false;
            EventManager.Instance.Unsubscribe<GameEvents.UnitDeathEvent>(this);
            EventManager.Instance.Unsubscribe<GameEvents.UnitMovementEvent>(this);
            UnitManager.Instance.UnitSystem.UnregisterUnit(RuntimeId);
        }

        public virtual void KillSelf()
        {
            if (!IsActive) return;
            CombatManager.Instance.ActorFactory.Despawn(this);
        }

        public virtual void OnEventReceived(GameEvents.UnitMovementEvent eventData)
        {
            if (!IsActive) return;
            if (eventData.GUID != RuntimeId) return;
            OnUpdateView?.Invoke(UnitData.Position, UnitData.Rotation);
        }

        public virtual void OnEventReceived(GameEvents.UnitDeathEvent eventData)
        {
            if (eventData.RuntimeId != RuntimeId) return;
            // 处理角色死亡逻辑
            KillSelf();
        }

        private void UpdatePosView(Vector2 position, float rotation)
        {
            transform.position = CoordinateConverter.ToWorldPosition(position);
            // Y轴取反, 因为逻辑坐标系是x-y平面, 而Unity是x-z平面
            transform.rotation = Quaternion.Euler(0, -rotation, 0);
        }
    }
}