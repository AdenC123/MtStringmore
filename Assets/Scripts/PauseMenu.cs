using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;

    public AudioMixer audioMixer;

    public Slider BGMSlider;
    public Slider SFXSlider;
    
    public float startBGMVolume = 0.5f;
    public float startSFXVolume = 0.5f;

    public void Start()
    {
        pauseMenuUI.SetActive(false);
        float savedBGMVolume = PlayerPrefs.GetFloat("BGM", startBGMVolume);
        float savedSFXVolume = PlayerPrefs.GetFloat("SFX", startSFXVolume);
        SetSFXVolume(savedSFXVolume);
        SetBGMVolume(savedBGMVolume);
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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;   
        float savedSFXVolume = PlayerPrefs.GetFloat("SFX", startSFXVolume);
        SetSFXVolume(savedSFXVolume);
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        
        audioMixer.SetFloat("SFX", -80f);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }
}
