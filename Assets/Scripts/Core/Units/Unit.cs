using System;
using Core.Events;
using Lean.Pool;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Units
{
    /// <summary>
    /// 单位对象基类
    /// </summary>
    public abstract class Unit : MonoBehaviour,
        IPoolable,
        IEventListener<GameEvents.UnitDeathEvent>,
        IEventListener<GameEvents.UnitMovementEvent>
    {
        public bool IsActive => UnitData.IsActive;
        public UnitData UnitData { get; protected set; }
        public int GUID => UnitData.GUID;
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
            UnitManager.Instance.UnitSystem.RegisterUnit(UnitData);
            Initialized = true;
        }

        private void Start()
        {
            if (!Initialized) return;
            Debug.Log("Unit Start");
            OnInitialize?.Invoke();
            OnUpdateView?.Invoke(UnitData.Position, UnitData.Rotation);
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
            UnitManager.Instance.UnitSystem.UnregisterUnit(GUID);
        }

        public virtual void KillSelf()
        {
            if (!IsActive) return;
            UnitManager.Instance.Factory.Despawn(this);
        }

        public virtual void OnEventReceived(GameEvents.UnitMovementEvent eventData)
        {
            if (!IsActive) return;
            if (eventData.GUID != GUID) return;
            OnUpdateView?.Invoke(UnitData.Position, UnitData.Rotation);
        }

        public virtual void OnEventReceived(GameEvents.UnitDeathEvent eventData)
        {
        }
    }
}