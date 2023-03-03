using System.Collections;
using UnityEngine;
using System;
using Cubes;
using UnityEngine.SceneManagement;
using System.IO;

public class LevelManager : MonoBehaviour
{

    public bool player1OnButton;
    public bool pushBlockOnButton;
    public string levelToUnlock;

    public static event Action gameEnded;
    public static LevelManager instance;
    public static bool sLevelEnded;
    public static string currentLevel;

    private bool _loaded;

    //assignment of values needed
    private void Start()
    {
        _loaded = false;
        sLevelEnded = false;
        instance = this;

        Player1CubeBehaviour.onButton += Player1OnButton;
        PushBlockBehaviour.onButton += PushBlockOnButton;

        currentLevel = SceneManager.GetActiveScene().name;
    }

    private void OnDestroy()
    {
        Player1CubeBehaviour.onButton -= Player1OnButton;
        PushBlockBehaviour.onButton -= PushBlockOnButton;
    }
    private void Update()
    {
        if (pushBlockOnButton && player1OnButton && !_loaded)
        {
            gameEnded?.Invoke();
            StartCoroutine(AsyncReturnToLevelSelect());
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            //Takes a screenShot of the current game view and saves it as a texture.
            StartCoroutine(ScreenShotStart());
        }
    }

    //checks if Player1 is on the end button.
    private void Player1OnButton(bool onButton)
    {
        player1OnButton = onButton;
    }

    //Checks if the push block is on the End button.
    private void PushBlockOnButton(bool onButton)
    {
        pushBlockOnButton = onButton;
    }


    //Loads the Level select screen and takes a screenshot of the finished level to place as the end texture in the level select.
    private IEnumerator AsyncReturnToLevelSelect()
    {
        _loaded = true;
        sLevelEnded = true;
        LevelStage.dontLoad = true;
        CubeController.sCanMove = false;
        PlayerPrefs.SetInt(levelToUnlock, 1);
        yield return new WaitForSeconds(1.2f);
        //take photo
        StartCoroutine(TakeScreenShot());
        AsyncOperation sceneToLoad =  SceneManager.LoadSceneAsync("Level Select", LoadSceneMode.Single);
        while (!sceneToLoad.isDone)
        {
            yield return new WaitForSeconds(0.1f);
        }

    }

    private IEnumerator ScreenShotStart()
    {
        yield return new WaitForEndOfFrame();

        int height = 768;
        int width = 1024;

        Texture2D ScreenShotTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        Rect rect = new Rect(0, 0, width, height);
        ScreenShotTexture.ReadPixels(rect, 0, 0);
        ScreenShotTexture.Apply();
        byte[] byteArray = ScreenShotTexture.EncodeToPNG();
        File.WriteAllBytes($"{Application.dataPath}/ScreenShots/{SceneManager.GetActiveScene().name} start.png", byteArray);
    }
    
    //takes a screenshot and saves it on the game files.
    private IEnumerator TakeScreenShot()
    {
        yield return new WaitForEndOfFrame();

        int height = 768;
        int width = 1024;

        Texture2D ScreenShotTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        Rect rect = new Rect(0, 0, width, height);
        ScreenShotTexture.ReadPixels(rect, 0, 0);
        ScreenShotTexture.Apply();
        byte[] byteArray = ScreenShotTexture.EncodeToPNG();
        File.WriteAllBytes($"{Application.persistentDataPath}/{SceneManager.GetActiveScene().name}.png", byteArray);
    }
}