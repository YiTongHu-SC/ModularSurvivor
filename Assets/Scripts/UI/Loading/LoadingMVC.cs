using UI.Framework;
using UnityEngine;

namespace UI.Loading
{
    public class LoadingMVC : MonoBehaviour
    {
        private LoadingView _loadingView;
        private LoadingController _loadingController;

        #region UNITY 生命周期

        private void Awake()
        {
            _loadingView = GetComponentInChildren<LoadingView>();
        }

        private void Start()
        {
            InitializeMVC();
        }

        private void OnDestroy()
        {
            if (MVCManager.Instance == null) return;
            MVCManager.Instance.UnregisterController(_loadingController);
            _loadingController = null;
        }

        #endregion

        private void InitializeMVC()
        {
            if (MVCManager.Instance == null) return;
            if (_loadingController != null && _loadingController.IsInitialized)
            {
                return;
            }

            // This method can be used for additional initialization if needed
            _loadingController = new LoadingController();
            var loadingModel = new LoadingModel();
            _loadingController.Initialize(loadingModel, _loadingView);
            MVCManager.Instance.RegisterController(_loadingController);
            Debug.Log("LoadingMVC Initialized");
        }
    }
}