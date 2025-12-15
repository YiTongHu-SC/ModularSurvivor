using Core.Events;
using UnityEngine;

namespace SceneTests.TestUI
{
    public class MainMenuUI : MonoBehaviour
    {
        public void StartGame()
        {
            Debug.Log("Start Game button clicked!");
            // Add logic to start the game
            EventManager.Instance.Publish(new GameEvents.GameStartEvent(0));
        }

        public void ExitGame()
        {
            Debug.Log("Exit Game button clicked!");
            // Add logic to exit the game
            EventManager.Instance.Publish(new GameEvents.GameExitEvent());
        }
    }
}