using UnityEngine;
using Cubes;

namespace UI
{
    public class PauseMenuController : UiController
    {
        public GameObject pauseMenu;
        public static bool sPaused;

        // Start is called before the first frame update
        private void Start()
        {
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
            sPaused = false;
            restarting += PauseCheck;
        }
        private void OnDestroy()
        {
            restarting -= PauseCheck;
        }


        //checks if the game is paused.
        public void PauseCheck()
        {
            if (sPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (LevelManager.sLevelEnded) { return; }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PauseCheck();
            }
        }

        //Resumes the game.
        public void Resume()
        {
            Time.timeScale = 1f;
            CubeController.sCanMove = true;
            pauseMenu.SetActive(false);
            sPaused = false;
        }

        //Pauses the game.
        public void Pause()
        {
            Time.timeScale = 0f;
            CubeController.sCanMove = false;
            pauseMenu.SetActive(true);
            sPaused = true;
        }
    }
}