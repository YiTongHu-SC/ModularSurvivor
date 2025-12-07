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
        public int ID { get; protected set; }
        public UnityEvent OnInitialize { get; set; }
        public UnityEvent<Vector2> OnUpdatePosition { get; set; }

        private void Awake()
        {
            Debug.Log("Unit Awake");
            OnInitialize = new UnityEvent();
            OnUpdatePosition = new UnityEvent<Vector2>();
        }

        public virtual void Initialize(UnitData data)
        {
            Debug.Log("Unit Initialize");
            ID = data.ID;
            UnitData = data;
            UnitData.IsActive = true;
            UnitManager.Instance.UnitSystem.RegisterUnit(UnitData);
        }

        private void Start()
        {
            Debug.Log("Unit Start");
            OnInitialize?.Invoke();
            OnUpdatePosition?.Invoke(UnitData.Position);
        }

        public virtual void OnSpawn()
        {
            Debug.Log("Unit Spawned");
            EventManager.Instance.Subscribe<GameEvents.UnitDeathEvent>(this);
            EventManager.Instance.Subscribe<GameEvents.UnitMovementEvent>(this);
        }

        public virtual void OnDespawn()
        {
            UnitData.IsActive = false;
            EventManager.Instance.Unsubscribe<GameEvents.UnitDeathEvent>(this);
            EventManager.Instance.Unsubscribe<GameEvents.UnitMovementEvent>(this);
            UnitManager.Instance.UnitSystem.UnregisterUnit(ID);
        }

        public virtual void KillSelf()
        {
            if (!IsActive) return;
            UnitManager.Instance.Factory.Despawn(this);
        }

        public virtual void OnEventReceived(GameEvents.UnitMovementEvent eventData)
        {
            if (!IsActive) return;
            if (eventData.UnitId != ID) return;
            SetPosition(eventData.UnitData.Position);
        }

        public virtual void OnEventReceived(GameEvents.UnitDeathEvent eventData)
        {
        }

        public virtual void SetPosition(Vector2 position)
        {
            UnitData.Position = position;
            OnUpdatePosition?.Invoke(UnitData.Position);
        }
    }
}