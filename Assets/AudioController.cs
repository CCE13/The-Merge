using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;

public class AudioController : MonoBehaviour
{
    public static AudioController instance;
    
    private AudioSource audioSource;
    public RawImage image;
    public Slider slider;

    public Texture mutedTex;
    public Texture soundTex;
    // Start is called before the first frame update
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void Start()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        image.texture = soundTex;

        //audioSource.volume = PlayerPrefs.GetFloat("volume");
        //Debug.Log(audioSource.volume);
        //slider.value = PlayerPrefs.GetFloat("volume");


        //slider.onValueChanged.AddListener(ValueChange);
        //Debug.Log(slider.value);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.buildIndex == 0)
        {
            
            slider = Resources.FindObjectsOfTypeAll<Slider>()[0];
            image = Resources.FindObjectsOfTypeAll<RawImage>()[0];
            if(audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
            audioSource.volume = PlayerPrefs.GetFloat("volume");
            slider.value = PlayerPrefs.GetFloat("volume");
            if (slider.value == 0f)
            {
                image.texture = mutedTex;
            }
            else
            {
                image.texture = soundTex;
            }

            slider.onValueChanged.AddListener(ValueChange);

        }
    }


    /// Sets the image to mute or sound depending on the slider value.
    /// <summary>
    /// </summary>
    public void ValueChange(float value)
    {
        audioSource.volume = value;
        if (value == 0f)
        {
            image.texture = mutedTex;
        }
        else
        {
            image.texture = soundTex;
        }
        PlayerPrefs.SetFloat("volume", audioSource.volume);
    }
    public void OnDisable()
    {
        PlayerPrefs.SetFloat("volume",audioSource.volume);
    }
}
