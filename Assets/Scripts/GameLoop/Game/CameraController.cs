using Combat.Systems;
using Core.Input;
using Core.Units;
using GameLoop.Config;
using UnityEngine;

namespace GameLoop.Game
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CameraConfig _config;
        private Camera _camera;
        private Vector3 _desiredPosition;
        private UnitData _playerUnit;
        private Transform _playerTarget;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            if (_config == null || !_playerTarget) return;

            // 更新相机参数
            // TODO: 后面可以放到一个初始化中，只更新一次
            _camera.fieldOfView = _config.FieldOfView;
            _camera.nearClipPlane = _config.NearClip;
            _camera.farClipPlane = _config.FarClip;
            // 计算目标位置
            // 相机俯视角：向上 CameraHeight，向后偏移（根据 Pitch 计算）
            float horizontalDist = _config.CameraHeight / Mathf.Tan(_config.PitchAngle * Mathf.Deg2Rad);

            _desiredPosition = Vector3.up * _config.CameraHeight
                               - Vector3.forward * horizontalDist
                               + Vector3.right * _config.CameraDistanceFromPlayer;

            // 平滑跟踪
            if (_config.FollowPlayer)
            {
                _desiredPosition += _playerTarget.position;
                transform.position = Vector3.Lerp(
                    transform.position,
                    _desiredPosition,
                    _config.FollowDamping
                );
                // 始终看向玩家
                transform.LookAt(_playerTarget.position + Vector3.up * _playerUnit.ModelView.CenterOffset);
            }
            else
            {
                transform.position = _desiredPosition;
            }
        }

        public void SetGlobalMainCamera()
        {
            InputManager.Instance.SetMainCamera(_camera);
            if (!_playerTarget)
            {
                _playerUnit = CombatManager.Instance.HeroActor.UnitData;
                _playerTarget = CombatManager.Instance.HeroActor.transform;
            }
        }
    }
}