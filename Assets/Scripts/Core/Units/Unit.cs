using System;
using System.Diagnostics.Tracing;
using Core.Events;
using Lean.Pool;
using UnityEngine;
using UnityEngine.EventSystems;

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
        public int ID;
        public UnitData UnitData;

        public virtual void Initialize(UnitData data)
        {
            ID = data.ID;
            UnitData = data;
            UnitManager.Instance.UnitSystem.RegisterUnit(UnitData);
        }

        public void Reset()
        {
        }

        public virtual void OnSpawn()
        {
            EventManager.Instance.Subscribe<GameEvents.UnitDeathEvent>(this);
            EventManager.Instance.Subscribe<GameEvents.UnitMovementEvent>(this);
        }

        public virtual void OnDespawn()
        {
            EventManager.Instance.Unsubscribe<GameEvents.UnitDeathEvent>(this);
            EventManager.Instance.Unsubscribe<GameEvents.UnitMovementEvent>(this);
        }

        public virtual void OnEventReceived(GameEvents.UnitMovementEvent eventData)
        {
        }

        public virtual void OnEventReceived(GameEvents.UnitDeathEvent eventData)
        {
        }
    }
}