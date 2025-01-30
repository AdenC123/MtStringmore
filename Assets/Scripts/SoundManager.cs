using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider BGMSlider;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private float startBGMVolume = 0.5f;
    [SerializeField] private float startSFXVolume = 0.5f;
    [SerializeField] private float startMasterVolume = 0.5f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        audioMixer.SetFloat("Master", startMasterVolume);
        var savedBGMVolume = PlayerPrefs.GetFloat("BGM", startBGMVolume);
        var savedSFXVolume = PlayerPrefs.GetFloat("SFX", startSFXVolume);
        SetBGMVolume(savedBGMVolume);
        SetSFXVolume(savedSFXVolume);
        BGMSlider.value = savedBGMVolume;
        SFXSlider.value = savedSFXVolume;
    }

    /// <summary> Sets BGM volume (0.0001 to 1). </summary>
    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("BGM", volume);
        PlayerPrefs.Save();
    }

    /// <summary> Sets SFX volume (0.0001 to 1). </summary>
    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFX", volume);
        PlayerPrefs.Save();
    }

    /// <summary> Mutes or unmutes all audio. </summary>
    public void SetMute(bool isMuted)
    {
        audioMixer.SetFloat("Master", isMuted ? -80f : 0f);
    }
}