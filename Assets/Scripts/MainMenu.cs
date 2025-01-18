using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider BGMSlider;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private float startBGMVolume = 0.5f;
    [SerializeField] private float startSFXVolume = 0.5f;
    [SerializeField] private float startMasterVolume = 0.5f;
    [SerializeField] private string startingScene;

    public void Start()
    {
        audioMixer.SetFloat("Master", startMasterVolume);
        var savedBGMVolume = PlayerPrefs.GetFloat("BGM", startBGMVolume);
        var savedSFXVolume = PlayerPrefs.GetFloat("SFX", startSFXVolume);
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
        SceneManager.LoadScene(startingScene);
    }

    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }
}