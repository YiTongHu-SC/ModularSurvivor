using System;
using Combat.Systems;
using Core.Input;
using StellarCore.Singleton;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Combat.GameCamera
{
    public class CameraManager : BaseInstance<CameraManager>
    {
        public Camera UICamera;
        public Camera BaseCamera;
        public Camera BattleCamera { get; set; }
        public BattleCameraController BattleCameraController { get; set; }

        public void Initialization()
        {
            Debug.Assert(UICamera != null, "UICamera is not assigned in CameraManager");
            Debug.Assert(BaseCamera != null, "BaseCamera is not assigned in CameraManager");
            CombatManager.Instance.CameraManager = this;
            InputManager.Instance.RegisterUICamera(UICamera);
            InputManager.Instance.RegisterBaseCamera(BaseCamera);
            var uiCamera = UICamera.GetUniversalAdditionalCameraData();
            uiCamera.renderType = CameraRenderType.Overlay;
            var baseCamera = BaseCamera.GetUniversalAdditionalCameraData();
            baseCamera.renderType = CameraRenderType.Base;
            baseCamera.cameraStack.Clear();
            baseCamera.cameraStack.Add(UICamera);
        }

        public void Reset()
        {
            BattleCamera = null;
            BattleCameraController = null;
            var uiCamera = UICamera.GetUniversalAdditionalCameraData();
            uiCamera.renderType = CameraRenderType.Overlay;
            var baseCamera = BaseCamera.GetUniversalAdditionalCameraData();
            baseCamera.renderType = CameraRenderType.Base;
            baseCamera.cameraStack.Clear();
            baseCamera.cameraStack.Add(UICamera);
        }

        public void SetBattleCamera(BattleCameraController battleCameraController)
        {
            BattleCameraController = battleCameraController;
            BattleCamera = BattleCameraController.Camera;
            InputManager.Instance.RegisterBattleCamera(BattleCamera);
            // battle camera setting
            BattleCamera.enabled = true;
            var battleCamera = BattleCamera.GetUniversalAdditionalCameraData();
            battleCamera.renderType = CameraRenderType.Overlay;
            var uiCamera = UICamera.GetUniversalAdditionalCameraData();
            uiCamera.renderType = CameraRenderType.Overlay;
            var baseCamera = BaseCamera.GetUniversalAdditionalCameraData();
            baseCamera.renderType = CameraRenderType.Base;
            baseCamera.cameraStack.Clear();
            baseCamera.cameraStack.Add(BattleCamera);
            baseCamera.cameraStack.Add(UICamera);
        }
    }
}