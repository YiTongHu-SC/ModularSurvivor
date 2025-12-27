using System;
using Combat.Config;
using Combat.Systems;
using Core.Events;
using Core.Units;
using UnityEngine;

namespace Combat.GameCamera
{
    public class BattleCameraController : MonoBehaviour,
        IEventListener<GameEvents.HeroCreated>
    {
        [SerializeField] private CameraConfig Config;
        public Camera Camera { get; private set; }
        private Vector3 _desiredPosition;
        private Transform _playerTarget;
        private float _offset;

        private void Awake()
        {
            Camera = GetComponent<Camera>();
            CameraManager.Instance.SetBattleCamera(this);
        }

        private void SetTarget(Transform target, float offset)
        {
            _playerTarget = target;
            _offset = offset;
        }

        private void LateUpdate()
        {
            if (Config == null || !_playerTarget) return;

            // 更新相机参数
            // TODO: 后面可以放到一个初始化中，只更新一次
            Camera.fieldOfView = Config.FieldOfView;
            Camera.nearClipPlane = Config.NearClip;
            Camera.farClipPlane = Config.FarClip;
            // 计算目标位置
            // 相机俯视角：向上 CameraHeight，向后偏移（根据 Pitch 计算）
            float horizontalDist = Config.CameraHeight / Mathf.Tan(Config.PitchAngle * Mathf.Deg2Rad);

            _desiredPosition = Vector3.up * Config.CameraHeight
                               - Vector3.forward * horizontalDist
                               + Vector3.right * Config.CameraDistanceFromPlayer;

            // 平滑跟踪
            if (Config.FollowPlayer)
            {
                _desiredPosition += _playerTarget.position;
                transform.position = Vector3.Lerp(
                    transform.position,
                    _desiredPosition,
                    Config.FollowDamping
                );
                // 始终看向玩家
                transform.LookAt(_playerTarget.position + Vector3.up * _offset);
            }
            else
            {
                transform.position = _desiredPosition;
            }
        }

        private void OnEnable()
        {
            EventManager.Instance.Subscribe<GameEvents.HeroCreated>(this);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe<GameEvents.HeroCreated>(this);
        }

        public void OnEventReceived(GameEvents.HeroCreated eventData)
        {
            if (UnitManager.Instance.TryGetAvailableUnit(eventData.HeroId, out var unitData))
            {
                SetTarget(eventData.HeroTransform, unitData.ModelView.CenterOffset);
            }
        }
    }
}