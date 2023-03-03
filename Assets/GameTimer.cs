using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private Text timeText;
    public bool startTimer;
    private float _secondsPast;
    private int _minutesPast;
    private float _millisecondsPast;

    public static GameTimer instance;
    public static float S_minutesEnded;
    public static float S_secondsEnded;
    public static float S_millisecondsEnded;
    private float _time;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        startTimer = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (LevelManager.sLevelEnded) { StoreTime(); }
        if (startTimer)
        {
            UpdateTime();
        }

    }
    private void UpdateTime()
    {
        _time += Time.deltaTime;
        _minutesPast = (int)(_time / 60f) % 60;
        _secondsPast = (int)(_time % 60f);
        _millisecondsPast = (int)(_time * 1000f) % 1000;
        timeText.text = $"{_minutesPast.ToString("00")}:{Mathf.FloorToInt(_secondsPast).ToString("00")}:{_millisecondsPast.ToString("00")}";
    }


    public void StoreTime()
    {
        S_secondsEnded = _secondsPast;
        S_minutesEnded = _minutesPast;
        S_millisecondsEnded = _millisecondsPast;
        startTimer = false;
    }
}
