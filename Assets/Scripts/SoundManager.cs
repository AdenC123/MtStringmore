using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider MasterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private float startBgmVolume = 0.5f;
    [SerializeField] private float startSfxVolume = 0.5f;
    [SerializeField] private float startMasterVolume = 0.5f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        var savedMasterVolume = PlayerPrefs.GetFloat("Master", startMasterVolume);
        var savedBgmVolume = PlayerPrefs.GetFloat("BGM", startBgmVolume);
        var savedSfxVolume = PlayerPrefs.GetFloat("SFX", startSfxVolume);
        SetMasterVolume(savedMasterVolume);
        SetBgmVolume(savedBgmVolume);
        SetSfxVolume(savedSfxVolume);
    }
    
    /// <summary> Sets Master volume (0.0001 to 1). </summary>
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        MasterSlider.value = volume;
        PlayerPrefs.SetFloat("Master", volume);
        PlayerPrefs.Save();
    }

    /// <summary> Sets BGM volume (0.0001 to 1). </summary>
    public void SetBgmVolume(float volume)
    {
        audioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
        bgmSlider.value = volume;
        PlayerPrefs.SetFloat("BGM", volume);
        PlayerPrefs.Save();
    }

    /// <summary> Sets SFX volume (0.0001 to 1). </summary>
    public void SetSfxVolume(float volume)
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        sfxSlider.value = volume;
        PlayerPrefs.SetFloat("SFX", volume);
        PlayerPrefs.Save();
    }

    /// <summary> Mutes or unmutes all audio. </summary>
    public void SetMute(bool isMuted)
    {
        audioMixer.SetFloat("Master", isMuted ? -80f : PlayerPrefs.GetFloat("Master", startMasterVolume));
    }
}