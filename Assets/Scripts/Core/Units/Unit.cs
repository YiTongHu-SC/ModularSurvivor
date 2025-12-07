using Core.Events;
using Lean.Pool;
using UnityEngine;

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

        public virtual void Initialize(UnitData data)
        {
            ID = data.ID;
            UnitData = data;
            UnitData.IsActive = true;
            SetPosition(data.Position);
            UnitManager.Instance.UnitSystem.RegisterUnit(UnitData);
        }

        public virtual void OnSpawn()
        {
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
        }

        public virtual void OnEventReceived(GameEvents.UnitDeathEvent eventData)
        {
        }

        public virtual void SetPosition(Vector2 position)
        {
            UnitData.Position = position;
            transform.position = position;
        }
    }
}