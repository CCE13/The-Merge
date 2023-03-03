using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class MainMenuController : UiController
    {
        public string sceneToLoad;
        public GameObject instructions;
        public GameObject credits;
        public GameObject settings;

        private AudioController _audio;

        private void Start()
        {
            instructions.SetActive(false);
            _audio = FindObjectOfType<AudioController>();
        }
        public void PlayGame()
        {
            SceneManager.LoadScene(sceneToLoad);
        }

        public void OpenInstructions()
        {
            instructions.SetActive(true);
        }
        public void OpenCredits()
        {
            credits.SetActive(true);
        }
        public void OpenSettings()
        {
            settings.SetActive(true);
            
        }

        public void BackToPreviousMenu()
        {
            if (instructions.activeInHierarchy)
            {
                instructions.SetActive(false);
            }
            if (credits.activeInHierarchy)
            {
                credits.SetActive(false);
            }
            if (settings.activeInHierarchy)
            {
                settings.SetActive(false);
            }
        }
    }

}

