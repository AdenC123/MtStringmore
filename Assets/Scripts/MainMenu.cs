using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{	
	public AudioMixer audioMixer;
	public Slider volumeSlider;
	public float startVolume = 0.5f;

	public void Start()
	{
		float savedVolume = PlayerPrefs.GetFloat("Volume", startVolume);
		SetVolume(savedVolume);
		volumeSlider.value = savedVolume;
	}

	public void SetVolume(float volume)
	{
		audioMixer.SetFloat("Volume", Mathf.Log10(volume) * 20);
		PlayerPrefs.SetFloat("Volume", volume);
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
