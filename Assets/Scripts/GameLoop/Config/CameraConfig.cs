using UnityEngine;

namespace GameLoop.Config
{
    [CreateAssetMenu(fileName = "CameraConfig", menuName = "Configs/CameraConfig", order = 0)]
    public class CameraConfig : ScriptableObject
    {
        [Header("相机位置")]
        [SerializeField] public float CameraHeight = 10f;           // 相对地面高度
        [SerializeField] public float CameraDistanceFromPlayer = 0f; // 水平偏移（通常为0中心）
        
        [Header("相机角度")]
        [SerializeField] public float PitchAngle = 50f;  // 俯视角度，范围 45-55
        // [SerializeField] public float YawAngle = 0f;     // 偏航（通常0，跟随玩家）
        
        [Header("相机视锥")]
        [SerializeField] public float FieldOfView = 55f; // 视野，范围 50-60
        [SerializeField] public float NearClip = 0.3f;
        [SerializeField] public float FarClip = 500f;
        
        [Header("相机跟踪")]
        [SerializeField] public bool FollowPlayer = true;
        [SerializeField] public float FollowDamping = 0.1f; // 跟踪平滑度 (0-1, 越小越平滑)
        // [SerializeField] public bool AllowUserPan = true; 
    }
}