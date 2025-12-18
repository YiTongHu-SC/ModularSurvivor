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
            MVCManager.Instance.Open<MainMenuController>();
        }
    }
}