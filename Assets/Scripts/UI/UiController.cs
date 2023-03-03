using UnityEngine;
using UnityEngine.SceneManagement;
using Cubes;
using System;

namespace UI
{
    public abstract class UiController : MonoBehaviour
    {
        public static event Action restarting;
        public void QuitGame()
        {
            Application.Quit();
            Debug.Log("Quitting");
        }

        public void BackToMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Main Menu");
            CubeController.sCanMove = true;
            MergingManager.S_IsMerged = false;
        }

        public void Restart()
        {
            restarting?.Invoke();
            Time.timeScale = 1f;
            CubeController.sCanMove = true;
            MergingManager.S_IsMerged = false;
        }
    }
}