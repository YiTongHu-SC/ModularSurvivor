using UI.Framework;
using UnityEngine;

namespace UI.Menus
{
    public class MainMenuManager : MonoBehaviour
    {
        private void Start()
        {
            StartMainMenu();
        }

        private void StartMainMenu()
        {
            MvcManager.Instance.Open<MainMenuController>();
        }

        private void OnDestroy()
        {
            MvcManager.Instance.DisposeUI<MainMenuController>();
        }
    }
}