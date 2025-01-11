using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{	
    public AudioMixer audioMixer;
    public Slider BGMSlider;
    public Slider SFXSlider;
    public float startBGMVolume = 0.5f;
    public float startSFXVolume = 0.5f;
    public float startMasterVolume = 0.5f;

    public void Start()
    {
        audioMixer.SetFloat("Master", startMasterVolume);
        float savedBGMVolume = PlayerPrefs.GetFloat("BGM", startBGMVolume);
        float savedSFXVolume = PlayerPrefs.GetFloat("SFX", startSFXVolume);
        SetBGMVolume(savedBGMVolume);
        SetSFXVolume(savedSFXVolume);
        BGMSlider.value = savedBGMVolume;
        SFXSlider.value = savedSFXVolume;
    }

    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("BGM", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFX", volume);
        PlayerPrefs.Save();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("PauseTest");
    }
    
    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }
}