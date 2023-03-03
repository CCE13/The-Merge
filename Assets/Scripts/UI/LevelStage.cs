using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using TMPro;

public class LevelStage : MonoBehaviour
{
    public string levelToLoad;
    public bool unlocked;
    public Camera mainCamera;
    private Texture finishedScreen;
    public Texture StartScreen;
    public TMP_Text LockedText;
    public bool haveDialogue;
    public TMP_Text timeText;
    public static bool dontLoad;


    private AsyncOperation _sceneToLoad;
    private bool _loaded;
    private RawImage _currentImage;
    private bool _changePicture;
    private bool _canChange;
    private Button _button;
    private LevelSelectSroll _scroll;
    private DialogueController _dialogue;
    private Animator animator;
    private void Awake()
    {
        mainCamera = FindObjectOfType<Camera>();
        _currentImage = transform.GetChild(1).GetComponent<RawImage>();
        _button = transform.GetChild(1).GetComponent<Button>();
        _scroll = FindObjectOfType<LevelSelectSroll>();
        _dialogue = GetComponent<DialogueController>();
        timeText = transform.GetChild(3).transform.GetChild(0).GetComponent<TMP_Text>();


    }
    private void OnValidate()
    {
        gameObject.name = levelToLoad;
    }

    private void Start()
    {
        _canChange = false;
        _changePicture = false;
        _currentImage.texture = StartScreen;
        float FastestMinutes = PlayerPrefs.GetFloat(levelToLoad + "Minutes");
        float FastestSeconds = PlayerPrefs.GetFloat(levelToLoad + "Seconds");
        float FastestMilleSeconds = PlayerPrefs.GetFloat(levelToLoad + "MilliSeconds");
        timeText.text = $"{FastestMinutes.ToString("00")}:{Mathf.FloorToInt(FastestSeconds).ToString("00")}:{FastestMilleSeconds.ToString("000")}";

        if (LevelManager.sLevelEnded && levelToLoad == LevelManager.currentLevel)
        {
            ResetLevelSelectScreen();
            LevelManager.currentLevel = null;
        }

        PlayerPrefs.SetInt("Tutorial", 1);

        if (PlayerPrefs.GetInt(levelToLoad) == 1)
        {
            LockedText.enabled = false;
            _button.interactable = true;
            unlocked = true;
        }
        else
        {
            LockedText.enabled = true;
            _button.interactable = false;
            unlocked = false;
        }

        _button.onClick.AddListener(() => LoadLevel());
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(() => LoadLevel());
    }

    private void Update()
    {
        if (_changePicture)
        {
            ColorChange(0f);

            StartCoroutine(ResetPicture());

            if (!_canChange) { return; }

            ColorChange(2f);
        }
    }

    //changes the transparency of the picture.
    private void ColorChange(float numberToReach)
    {
        var tempColor = _currentImage.color;
        tempColor.a = numberToReach;
        _currentImage.color = Color.Lerp(_currentImage.color, tempColor, 0.1f);
    }

    //loadsthe level to load.
    private void LoadLevel()
    {
        if (!unlocked) { return; }
        if (dontLoad) { return; }
        if (_loaded) { return; }
        if (DialogueController.isRunning) { return; }
        StartCoroutine(ZoomInTransition());
    }

    //resets the level select when the player returns to it.
    private void ResetLevelSelectScreen()
    {
        StartCoroutine(ZoomOutTransition());
        StartCoroutine(ResetValues());
    }

    //loads the scene in the background.
    private IEnumerator Loading(string LevelToLoad)
    {
        _sceneToLoad = SceneManager.LoadSceneAsync(LevelToLoad, LoadSceneMode.Single);
        _loaded = true;
        while (!_sceneToLoad.isDone)
        {
            yield return null;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(LevelToLoad));
    }

    //changes the image to the starting image.
    private IEnumerator ResetPicture()
    {
        yield return new WaitForSeconds(1f);
        _currentImage.texture = StartScreen;
        _canChange = true;
    }

    //resets te values when the camera zooms out.
    private IEnumerator ResetValues()
    { 
        yield return new WaitForSeconds(0.5f);
        dontLoad = false;
        _loaded = false;
        _changePicture = true;
        LevelManager.sLevelEnded = false;
        yield return new WaitForSeconds(1.5f);
        StopAllCoroutines();
    }
    
    //zooms out when the player finishes the game.
    private IEnumerator ZoomOutTransition()
    {
        mainCamera.fieldOfView = 43f;
        SetScreenShot();
        _currentImage.texture = finishedScreen;

        float currentFOV = mainCamera.fieldOfView;
        float targetFOV = 77f;

        while (currentFOV < targetFOV)
        {
            mainCamera.fieldOfView = Mathf.MoveTowards(currentFOV, targetFOV, Time.deltaTime * 160);
            currentFOV = mainCamera.fieldOfView;
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitUntil(() => mainCamera.fieldOfView >= 76.9f);

        if (haveDialogue)
        {
            _dialogue.StartDialogue();
            
        }
        SetTime();

        _scroll.NextLevel();

    }


    //sets the best time when the player finishes the level.
    private void SetTime()
    {

        float FastestMinutes = PlayerPrefs.GetFloat(levelToLoad + "Minutes");
        float FastestSeconds = PlayerPrefs.GetFloat(levelToLoad + "Seconds");
        float FastestMilleSeconds = PlayerPrefs.GetFloat(levelToLoad + "MilliSeconds");
        if(FastestMilleSeconds == 0 && FastestMinutes == 0 && FastestSeconds == 0)
        {
            PlayerPrefs.SetFloat(levelToLoad + "Minutes", GameTimer.S_minutesEnded);
            PlayerPrefs.SetFloat(levelToLoad + "Seconds", GameTimer.S_secondsEnded);
            PlayerPrefs.SetFloat(levelToLoad + "MilliSeconds", GameTimer.S_millisecondsEnded);
            timeText.text = $"{GameTimer.S_minutesEnded.ToString("00")}:{Mathf.FloorToInt(GameTimer.S_secondsEnded).ToString("00")}:{GameTimer.S_millisecondsEnded.ToString("000")}";
            return;
        }
        if(GameTimer.S_minutesEnded < FastestMinutes)
        {
            //is faster
            PlayerPrefs.SetFloat(levelToLoad + "Minutes", GameTimer.S_minutesEnded);
            PlayerPrefs.SetFloat(levelToLoad + "Seconds", GameTimer.S_secondsEnded);
            PlayerPrefs.SetFloat(levelToLoad + "MilliSeconds", GameTimer.S_millisecondsEnded);
            timeText.text = $"{GameTimer.S_minutesEnded.ToString("00")}:{Mathf.FloorToInt(GameTimer.S_secondsEnded).ToString("00")}:{GameTimer.S_millisecondsEnded.ToString("000")}";
            return;
        }
        if(GameTimer.S_minutesEnded == FastestMinutes)
        {
            if(GameTimer.S_secondsEnded < FastestSeconds)
            {
                PlayerPrefs.SetFloat(levelToLoad + "Seconds", GameTimer.S_secondsEnded);
                PlayerPrefs.SetFloat(levelToLoad + "MilliSeconds", GameTimer.S_millisecondsEnded);
                timeText.text = $"{FastestMinutes.ToString("00")}:{Mathf.FloorToInt(GameTimer.S_secondsEnded).ToString("00")}:{GameTimer.S_millisecondsEnded.ToString("000")}";
            }
            else
            {
                timeText.text = $"{FastestMinutes.ToString("00")}:{Mathf.FloorToInt(FastestSeconds).ToString("00")}:{FastestMilleSeconds.ToString("000")}";
            }
            return;
        }
        if(GameTimer.S_minutesEnded == FastestMinutes && GameTimer.S_secondsEnded == FastestSeconds)
        {
            if(GameTimer.S_millisecondsEnded < FastestMilleSeconds)
            {
                PlayerPrefs.SetFloat(levelToLoad + "MilliSeconds", GameTimer.S_millisecondsEnded);
                timeText.text = $"{FastestMinutes.ToString("00")}:{Mathf.FloorToInt(FastestSeconds).ToString("00")}:{GameTimer.S_millisecondsEnded.ToString("000")}";
            }
            else
            {
                timeText.text = $"{FastestMinutes.ToString("00")}:{Mathf.FloorToInt(FastestSeconds).ToString("00")}:{FastestMilleSeconds.ToString("000")}";
            }
            return;

        }
    }


    //gets the screenshot file and sets it as the final trexture.
    private void SetScreenShot()
    {
        if (File.Exists($"{Application.persistentDataPath}/{levelToLoad}.png"))
        {
            byte[] fileData = File.ReadAllBytes($"{Application.persistentDataPath}/{levelToLoad}.png");
            Texture2D ScreenShotTextures = new Texture2D(1920, 1080, TextureFormat.ARGB32, false);
            ScreenShotTextures.LoadImage(fileData);
            finishedScreen = ScreenShotTextures;
        }
        
    }


    //zooms into the image.
    private IEnumerator ZoomInTransition()
    {
        float currentFOV = mainCamera.fieldOfView;
        float targetFOV = 43f;

        while (currentFOV > targetFOV)
        {
            mainCamera.fieldOfView = Mathf.MoveTowards(currentFOV, targetFOV, Time.deltaTime * 160);
            currentFOV = mainCamera.fieldOfView;
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitUntil(() => mainCamera.fieldOfView <= 56f);
        mainCamera.fieldOfView = targetFOV;
        StartCoroutine(Loading(levelToLoad));

    }
}