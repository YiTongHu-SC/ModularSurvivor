using Combat.Config;
using Combat.Systems;
using Core.Input;
using UnityEngine;

namespace Combat.GameCamera
{
    public class BattleCameraController : MonoBehaviour
    {
        [SerializeField] private CameraConfig Config;
        private Camera _camera;
        private Vector3 _desiredPosition;
        private Transform _playerTarget;
        private float _offset;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        private void Start()
        {
            CombatManager.Instance.CameraManager.SetBattleCamera(_camera);
        }

        public void SetTarget(Transform target, float offset)
        {
            _playerTarget = target;
            _offset = offset;
        }

        private void LateUpdate()
        {
            if (Config == null || !_playerTarget) return;

            // 更新相机参数
            // TODO: 后面可以放到一个初始化中，只更新一次
            _camera.fieldOfView = Config.FieldOfView;
            _camera.nearClipPlane = Config.NearClip;
            _camera.farClipPlane = Config.FarClip;
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
    }
}